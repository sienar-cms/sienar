using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Infrastructure.Services;

public class EntityDeleter<TEntity, TContext>
	: DbService<TEntity, TContext>, IEntityDeleter<TEntity>
	where TEntity : EntityBase
	where TContext : DbContext
{
	private readonly IEnumerable<IBeforeDelete<TEntity>> _beforeDeleteHooks;
	private readonly IEnumerable<IAfterDelete<TEntity>> _afterDeleteHooks;

	/// <inheritdoc />
	public EntityDeleter(
		IDbContextAccessor<TContext> contextAccessor,
		ILogger<DbService<TEntity, TContext>> logger,
		INotificationService notifier,
		IEnumerable<IBeforeDelete<TEntity>> beforeDeleteHooks,
		IEnumerable<IAfterDelete<TEntity>> afterDeleteHooks)
		: base(contextAccessor, logger, notifier)
	{
		_beforeDeleteHooks = beforeDeleteHooks;
		_afterDeleteHooks = afterDeleteHooks;
	}

	/// <inheritdoc />
	public async Task<bool> Delete(Guid id)
	{
		TEntity? entity;
		try
		{
			entity = await EntitySet.FindAsync(id);
			if (entity is null)
			{
				Notifier.Error(StatusMessages.Crud<TEntity>.NotFound(id));
				return false;
			}
		}
		catch (Exception e)
		{
			Logger.LogError(e, StatusMessages.Database.QueryFailed);
			Notifier.Error(StatusMessages.Crud<TEntity>.DeleteFailed());
			return false;
		}

		var successful = true;
		try
		{
			foreach (var beforeHook in _beforeDeleteHooks)
			{
				var status = await beforeHook.Handle(entity);
				if (status != HookStatus.Success) successful = false;
			}
		}
		catch (Exception e)
		{
			Logger.LogError(e, "One or more before delete hooks failed to run");
			successful = false;
		}

		if (!successful)
		{
			Notifier.Error(StatusMessages.Crud<TEntity>.DeleteFailed());
			return false;
		}

		try
		{
			EntitySet.Remove(entity);
			await Context.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Logger.LogError(e, StatusMessages.Database.QueryFailed);
			Notifier.Error(StatusMessages.Crud<TEntity>.DeleteFailed());
			return false;
		}

		successful = true;
		try
		{
			foreach (var afterHook in _afterDeleteHooks)
			{
				if (await afterHook.Handle(entity) != HookStatus.Success) successful = false;
			}
		}
		catch (Exception e)
		{
			Logger.LogError(e, "One or more after delete hooks failed to run");
			return false;
		}

		if (!successful)
		{
			Notifier.Error(StatusMessages.Crud<TEntity>.DeleteFailed());
			return false;
		}

		Notifier.Success(StatusMessages.Crud<TEntity>.DeleteSuccessful());
		return successful;
	}
}

public class EntityDeleter<TEntity> : EntityDeleter<TEntity, DbContext>
	where TEntity : EntityBase
{
	/// <inheritdoc />
	public EntityDeleter(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<DbService<TEntity, DbContext>> logger,
		INotificationService notifier,
		IEnumerable<IBeforeDelete<TEntity>> beforeDeleteHooks,
		IEnumerable<IAfterDelete<TEntity>> afterDeleteHooks)
		: base(
			contextAccessor,
			logger,
			notifier,
			beforeDeleteHooks,
			afterDeleteHooks) {}
}