using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Extensions;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Infrastructure.Services;

public class EntityReader<TEntity, TContext> : DbService<TEntity, TContext>,
	IEntityReader<TEntity>
	where TEntity : EntityBase, new()
	where TContext : DbContext
{
	private readonly IFilterProcessor<TEntity> _filterProcessor;
	private readonly IEnumerable<IAccessValidator<TEntity>> _accessValidators;
	private readonly IEnumerable<IAfterProcess<TEntity>> _afterHooks;

	/// <inheritdoc />
	public EntityReader(
		IDbContextAccessor<TContext> contextAccessor,
		ILogger<EntityReader<TEntity, TContext>> logger,
		INotificationService notifier,
		IFilterProcessor<TEntity> filterProcessor,
		IEnumerable<IAccessValidator<TEntity>> accessValidators,
		IEnumerable<IAfterProcess<TEntity>> afterHooks)
		: base(contextAccessor, logger, notifier)
	{
		_filterProcessor = filterProcessor;
		_accessValidators = accessValidators;
		_afterHooks = afterHooks;
	}

	/// <inheritdoc />
	public async Task<TEntity?> Read(
		Guid id,
		Filter? filter = null)
	{
		TEntity? entity;
		try
		{
			entity = filter == null
				? await EntitySet.FindAsync(id)
				: await _filterProcessor
					.ProcessIncludes(EntitySet, filter)
					.FirstOrDefaultAsync(u => u.Id == id);
		}
		catch (Exception e)
		{
			Logger.LogError(e, StatusMessages.Database.QueryFailed);
			Notifier.Error(StatusMessages.Crud<TEntity>.ReadSingleFailed());
			return null;
		}

		if (entity is null)
		{
			Notifier.Error(StatusMessages.Crud<TEntity>.NotFound(id));
			return null;
		}

		if (!await _accessValidators.Validate(entity, ActionType.Read, Logger))
		{
			Notifier.Error(StatusMessages.Crud<TEntity>.NoPermission());
			return null;
		}

		await _afterHooks.Run(entity, ActionType.Read, Logger);
		return entity;
	}

	/// <inheritdoc />
	public async Task<PagedQuery<TEntity>> Read(Filter? filter = null)
	{
		IEnumerable<TEntity> buffered;
		int count;

		try
		{
			IQueryable<TEntity> entries;
			IQueryable<TEntity> countEntries;

			if (filter is not null)
			{
				entries = ProcessFilter(filter);
				countEntries = _filterProcessor.Search(EntitySet, filter);
			}
			else
			{
				entries = EntitySet;
				countEntries = EntitySet;
			}

			buffered = await entries.ToListAsync();
			count = await countEntries.CountAsync();
		}
		catch (Exception e)
		{
			Logger.LogError(e, StatusMessages.Database.QueryFailed);
			Notifier.Error(StatusMessages.Crud<TEntity>.ReadMultipleFailed());
			return new();
		}

		foreach (var entity in buffered)
		{
			// TODO: run IAccessValidators and remove entities for which validation fails
			await _afterHooks.Run(entity, ActionType.ReadAll, Logger);
		}

		return new (buffered, count);
	}

	private IQueryable<TEntity> ProcessFilter(
		Filter filter,
		Expression<Func<TEntity, bool>>? predicate = null)
	{
		var result = (IQueryable<TEntity>)EntitySet;
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

public class EntityReader<TEntity> : EntityReader<TEntity, DbContext>
	where TEntity : EntityBase, new()
{
	/// <inheritdoc />
	public EntityReader(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<EntityReader<TEntity, DbContext>> logger,
		INotificationService notifier,
		IFilterProcessor<TEntity> filterProcessor,
		IEnumerable<IAccessValidator<TEntity>> accessValidators,
		IEnumerable<IAfterProcess<TEntity>> afterHooks)
		: base(
			contextAccessor,
			logger,
			notifier,
			filterProcessor,
			accessValidators, 
			afterHooks) {}
}