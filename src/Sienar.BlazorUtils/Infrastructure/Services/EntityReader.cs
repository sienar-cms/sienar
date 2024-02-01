using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Infrastructure.Services;

public class EntityReader<TEntity, TContext>
	: DbService<TEntity, TContext>, IEntityReader<TEntity>
	where TEntity : EntityBase, new()
	where TContext : DbContext
{
	private const string BeforeReadHookErrorMessage = "One or more before read hooks failed to run";
	private const string AfterReadHookErrorMessage = "One or more after read hooks failed to run";

	private readonly IFilterProcessor<TEntity> _filterProcessor;
	private readonly IEnumerable<IBeforeRead<TEntity>> _beforeReadHooks;
	private readonly IEnumerable<IAfterRead<TEntity>> _afterReadHooks;

	/// <inheritdoc />
	public EntityReader(
		IDbContextAccessor<TContext> contextAccessor,
		ILogger<EntityReader<TEntity, TContext>> logger,
		INotificationService notifier,
		IFilterProcessor<TEntity> filterProcessor,
		IEnumerable<IBeforeRead<TEntity>> beforeReadHooks,
		IEnumerable<IAfterRead<TEntity>> afterReadHooks)
		: base(contextAccessor, logger, notifier)
	{
		_filterProcessor = filterProcessor;
		_beforeReadHooks = beforeReadHooks;
		_afterReadHooks = afterReadHooks;
	}

	/// <inheritdoc />
	public async Task<TEntity?> Get(
		Guid id,
		Filter? filter = null)
	{
		(var successful, filter) = RunBeforeHooks(filter, true);
		if (!successful)
		{
			Notifier.Error(StatusMessages.Crud<TEntity>.ReadSingleFailed());
			return null;
		}

		TEntity? entity = null;
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
		}

		if (entity is null)
		{
			Notifier.Error(StatusMessages.Crud<TEntity>.NotFound(id));
			return null;
		}

		if (!await RunAfterHooks([entity], true))
		{
			Notifier.Error(StatusMessages.Crud<TEntity>.ReadSingleFailed());
			return null;
		}

		return entity;
	}

	/// <inheritdoc />
	public async Task<PagedQuery<TEntity>> Get(Filter? filter = null)
	{
		IQueryable<TEntity> entries;
		IQueryable<TEntity> countEntries;
		IEnumerable<TEntity> buffered;
		int count;

		(var successful, filter) = RunBeforeHooks(filter, false);
		if (!successful)
		{
			Notifier.Error(StatusMessages.Crud<TEntity>.ReadMultipleFailed());
			return new();
		}

		try
		{
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

		if (!await RunAfterHooks(buffered, false))
		{
			Notifier.Error(StatusMessages.Crud<TEntity>.ReadMultipleFailed());
			return new();
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

	private (bool, Filter?) RunBeforeHooks(Filter? filter, bool isSingle)
	{
		try
		{
			foreach (var beforeReadHook in _beforeReadHooks)
			{
				filter = beforeReadHook.Handle(filter, isSingle);
			}
		}
		catch (Exception e)
		{
			Logger.LogError(e, BeforeReadHookErrorMessage);
			return (false, filter);
		}

		return (true, filter);
	}

	private async Task<bool> RunAfterHooks(IEnumerable<TEntity> entries, bool isSingle)
	{
		var successful = true;

		try
		{
			foreach (var entry in entries)
			{
				foreach (var afterReadHook in _afterReadHooks)
				{
					if (await afterReadHook.Handle(entry, isSingle) != HookStatus.Success) successful = false;
				}
			}
		}
		catch (Exception e)
		{
			Logger.LogError(e, AfterReadHookErrorMessage);
			return false;
		}

		return successful;
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
		IEnumerable<IBeforeRead<TEntity>> beforeReadHooks,
		IEnumerable<IAfterRead<TEntity>> afterReadHooks)
		: base(
			contextAccessor,
			logger,
			notifier,
			filterProcessor,
			beforeReadHooks, 
			afterReadHooks) {}
}