#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Data;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Services;

/// <exclude />
public class EntityReader<TEntity, TContext> : ServiceBase, IEntityReader<TEntity>
	where TEntity : EntityBase, new()
	where TContext : DbContext
{
	private readonly TContext _context;
	private readonly IFilterProcessor<TEntity> _filterProcessor;
	private readonly ILogger<EntityReader<TEntity, TContext>> _logger;
	private readonly IAccessValidatorService<TEntity> _accessValidator;
	private readonly IAfterActionService<TEntity> _afterHooks;

	private DbSet<TEntity> EntitySet => _context.Set<TEntity>();

	public EntityReader(
		INotificationService notifier,
		TContext context,
		IFilterProcessor<TEntity> filterProcessor,
		ILogger<EntityReader<TEntity, TContext>> logger,
		IAccessValidatorService<TEntity> accessValidator,
		IAfterActionService<TEntity> afterHooks)
		: base(notifier)
	{
		_context = context;
		_filterProcessor = filterProcessor;
		_logger = logger;
		_accessValidator = accessValidator;
		_afterHooks = afterHooks;
	}

	public async Task<OperationResult<TEntity?>> Read(
		Guid id,
		Filter? filter = null)
	{
		TEntity? entity;
		try
		{
			filter = _filterProcessor.ModifyFilter(filter, ActionType.Read);
			entity = filter is null
				? await EntitySet.FindAsync(id)
				: await _filterProcessor
					.ProcessIncludes(EntitySet, filter)
					.FirstOrDefaultAsync(e => e.Id == id);
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return NotifyOfResult(new OperationResult<TEntity?>(
				OperationStatus.Unknown,
				null,
				StatusMessages.Crud<TEntity>.ReadSingleFailed()));
		}

		if (entity is null)
		{
			return NotifyOfResult(new OperationResult<TEntity?>(
				OperationStatus.NotFound,
				null,
				StatusMessages.Crud<TEntity>.NotFound(id)));
		}

		// Run access validation
		var accessValidationResult = await _accessValidator.Validate(entity, ActionType.Read);
		if (!accessValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<TEntity?>(
				OperationStatus.Unauthorized,
				null,
				StatusMessages.Crud<TEntity>.NoPermission()));
		}

		await _afterHooks.Run(entity, ActionType.Read);
		return NotifyOfResult(new OperationResult<TEntity?>(result: entity));
	}

	public async Task<OperationResult<PagedQuery<TEntity>>> Read(Filter? filter = null)
	{
		PagedQuery<TEntity> queryResult;
		IQueryable<TEntity> entities = EntitySet;
		IQueryable<TEntity> countEntities = EntitySet;

		try
		{
			if (filter is not null)
			{
				entities = ProcessFilter(filter);
				countEntities = _filterProcessor.Search(countEntities, filter);
			}
			queryResult = new PagedQuery<TEntity>(
				await entities.ToListAsync(),
				await countEntities.CountAsync());
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return NotifyOfResult(new OperationResult<PagedQuery<TEntity>>(
				OperationStatus.Unknown,
				new PagedQuery<TEntity>(),
				StatusMessages.Crud<TEntity>.ReadMultipleFailed()));
		}

		foreach (var entity in queryResult.Items)
		{
			await _afterHooks.Run(entity, ActionType.ReadAll);
		}

		return NotifyOfResult(new OperationResult<PagedQuery<TEntity>>(result: queryResult));
	}

	private IQueryable<TEntity> ProcessFilter(
		Filter filter,
		Expression<Func<TEntity, bool>>? predicate = null)
	{
		IQueryable<TEntity> result = EntitySet;

		if (predicate is not null)
		{
			result = result.Where(predicate);
		}

		result = _filterProcessor.Search(result, filter);
		result = _filterProcessor.ProcessIncludes(result, filter);
		var sortPredicate = _filterProcessor.GetSortPredicate(filter.SortName);
		result = filter.SortDescending ?? false
			? result.OrderByDescending(sortPredicate)
			: result.OrderBy(sortPredicate);

		if (filter.Page > 1)
		{
			result = result.Skip((filter.Page - 1) * filter.PageSize);
		}

		// If filter.PageSize == 0, return all results
		if (filter.PageSize > 0)
		{
			result = result.Take(filter.PageSize);
		}

		return result;
	}
}