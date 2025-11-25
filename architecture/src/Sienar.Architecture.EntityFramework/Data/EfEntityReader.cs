using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Processors;
using Sienar.Security;
using Sienar.Services;

namespace Sienar.Data;

/// <summary>
/// An implementation of <see cref="IEntityReader{TEntity}"/> which reads entities from an EntityFramework <see cref="DbContext"/>
/// </summary>
/// <typeparam name="TEntity">The type of the entity to read</typeparam>
/// <typeparam name="TContext">The type of the app's <see cref="DbContext"/></typeparam>
public class EfEntityReader<TEntity, TContext> : ServiceBase, IEntityReader<TEntity>
	where TEntity : EntityBase
	where TContext : DbContext
{
	private readonly TContext _context;
	private readonly IEntityFrameworkFilterProcessor<TEntity> _filterProcessor;
	private readonly ILogger<EfEntityReader<TEntity, TContext>> _logger;
	private readonly IAccessValidationRunner<TEntity> _accessValidationRunner;
	private readonly IAfterActionRunner<TEntity> _afterActionRunner;

	public EfEntityReader(
		INotifier notifier,
		TContext context,
		IEntityFrameworkFilterProcessor<TEntity> filterProcessor,
		ILogger<EfEntityReader<TEntity, TContext>> logger,
		IAccessValidationRunner<TEntity> accessValidationRunner,
		IAfterActionRunner<TEntity> afterActionRunner)
		: base(notifier)
	{
		_context = context;
		_filterProcessor = filterProcessor;
		_logger = logger;
		_accessValidationRunner = accessValidationRunner;
		_afterActionRunner = afterActionRunner;
	}

	/// <inheritdoc />
	public async Task<OperationResult<TEntity?>> Read(
		int id,
		Filter? filter = null)
	{
		TEntity? entity;
		var entitySet = _context.Set<TEntity>();
		filter = _filterProcessor.ModifyFilter(filter, ActionType.Read);

		try
		{
			entity = filter is null
				? await entitySet.FindAsync(id)
				: await _filterProcessor
					.ProcessIncludes(entitySet, filter)
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
		var accessValidationResult = await _accessValidationRunner.Validate(entity, ActionType.Read);
		if (!accessValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<TEntity?>(
				OperationStatus.Unauthorized,
				null,
				StatusMessages.Crud<TEntity>.NoPermission()));
		}

		await _afterActionRunner.Run(entity, ActionType.Read);
		return NotifyOfResult(new OperationResult<TEntity?>(result: entity));
	}

	/// <inheritdoc />
	public async Task<OperationResult<PagedQueryResult<TEntity>>> Read(Filter? filter = null)
	{
		PagedQueryResult<TEntity> queryResult;

		try
		{
			filter = _filterProcessor.ModifyFilter(filter, ActionType.ReadAll);
			var entitySet = _context.Set<TEntity>();
			IQueryable<TEntity> entries;
			IQueryable<TEntity> countEntries;

			if (filter is not null)
			{
				entries = ProcessFilter(filter);
				countEntries = _filterProcessor.Search(entitySet, filter);
			}
			else
			{
				entries = entitySet;
				countEntries = entitySet;
			}

			queryResult = new PagedQueryResult<TEntity>(
				await entries.ToListAsync(),
				await countEntries.CountAsync());
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return NotifyOfResult(new OperationResult<PagedQueryResult<TEntity>>(
				OperationStatus.Unknown,
				new PagedQueryResult<TEntity>(),
				StatusMessages.Crud<TEntity>.ReadMultipleFailed()));
		}

		foreach (var entity in queryResult.Items)
		{
			await _afterActionRunner.Run(entity, ActionType.ReadAll);
		}

		return NotifyOfResult(new OperationResult<PagedQueryResult<TEntity>>(result: queryResult));
	}

	private IQueryable<TEntity> ProcessFilter(
		Filter filter,
		Expression<Func<TEntity, bool>>? predicate = null)
	{
		var result = (IQueryable<TEntity>) _context.Set<TEntity>();
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
