using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Infrastructure.Services;

public class EntityReader<TEntity, TContext>
	: DbService<TEntity, TContext>, IEntityReader<TEntity>
	where TEntity : EntityBase, new()
	where TContext : DbContext
{
	protected const string BeforeReadHookErrorMessage = "One or more before read hooks failed to run";
	protected const string AfterReadHookErrorMessage = "One or more after read hooks failed to run";

	protected readonly IFilterProcessor<TEntity> FilterProcessor;
	protected readonly IEnumerable<IBeforeRead<TEntity>> BeforeReadHooks;
	protected readonly IEnumerable<IAfterRead<TEntity>> AfterReadHooks;

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
		FilterProcessor = filterProcessor;
		BeforeReadHooks = beforeReadHooks;
		AfterReadHooks = afterReadHooks;
	}

	/// <inheritdoc />
	public virtual async Task<TEntity?> Get(
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
				: await FilterProcessor
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
	public virtual async Task<PagedDto<TEntity>> Get(Filter? filter = null)
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
				countEntries = FilterProcessor.Search(EntitySet, filter);
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

	protected IQueryable<TEntity> ProcessFilter(
		Filter filter,
		Expression<Func<TEntity, bool>>? predicate = null)
	{
		var result = (IQueryable<TEntity>)EntitySet;
		if (predicate is not null)
		{
			result = result.Where(predicate);
		}

		result = FilterProcessor.Search(result, filter);
		result = FilterProcessor.ProcessIncludes(result, filter);
		var sortPredicate = FilterProcessor.GetSortPredicate(filter.SortName);
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

	protected (bool, Filter?) RunBeforeHooks(Filter? filter, bool isSingle)
	{
		try
		{
			foreach (var beforeReadHook in BeforeReadHooks)
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

	protected async Task<bool> RunAfterHooks(IEnumerable<TEntity> entries, bool isSingle)
	{
		var successful = true;

		try
		{
			foreach (var entry in entries)
			{
				foreach (var afterReadHook in AfterReadHooks)
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