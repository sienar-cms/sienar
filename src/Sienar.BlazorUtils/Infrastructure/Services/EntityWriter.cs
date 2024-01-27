using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Infrastructure.Services;

public class EntityWriter<TEntity, TContext> : DbService<TEntity, TContext>, IEntityWriter<TEntity>
	where TEntity : EntityBase, new()
	where TContext : DbContext
{
	protected readonly IEnumerable<IEntityStateValidator<TEntity>> StateValidators;
	protected readonly IEnumerable<IBeforeUpsert<TEntity>> BeforeUpsertHooks;
	protected readonly IEnumerable<IAfterUpsert<TEntity>> AfterUpsertHooks;

	/// <inheritdoc />
	public EntityWriter(
		IDbContextAccessor<TContext> contextAccessor,
		ILogger<DbService<TEntity, TContext>> logger,
		INotificationService notifier,
		IEnumerable<IEntityStateValidator<TEntity>> stateValidators,
		IEnumerable<IBeforeUpsert<TEntity>> beforeUpsertHooks,
		IEnumerable<IAfterUpsert<TEntity>> afterUpsertHooks)
		: base(contextAccessor, logger, notifier)
	{
		StateValidators = stateValidators;
		BeforeUpsertHooks = beforeUpsertHooks;
		AfterUpsertHooks = afterUpsertHooks;
	}

	/// <inheritdoc />
	public virtual async Task<Guid> Add(TEntity model)
	{
		if (!await RunBeforeHooks(model, true))
		{
			Notifier.Error(StatusMessages.Crud<TEntity>.CreateFailed());
			return Guid.Empty;
		}

		try
		{
			await EntitySet.AddAsync(model);
			await Context.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Logger.LogError(e, StatusMessages.Database.QueryFailed);
			Notifier.Error(StatusMessages.Crud<TEntity>.CreateFailed());
			return Guid.Empty;
		}

		if (!await RunAfterHooks(model, true))
		{
			Notifier.Error(StatusMessages.Crud<TEntity>.CreateFailed());
			return Guid.Empty;
		}

		Notifier.Success(StatusMessages.Crud<TEntity>.CreateSuccessful());
		return model.Id;
	}

	/// <inheritdoc />
	public virtual async Task<bool> Edit(TEntity model)
	{
		if (!await RunBeforeHooks(model, false))
		{
			Notifier.Error(StatusMessages.Crud<TEntity>.UpdateFailed());
			return false;
		}

		try
		{
			EntitySet.Update(model);
			await Context.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Logger.LogError(e, StatusMessages.Database.QueryFailed);
			Notifier.Error(StatusMessages.Crud<TEntity>.UpdateFailed());
			return false;
		}

		if (!await RunAfterHooks(model, false))
		{
			Notifier.Error(StatusMessages.Crud<TEntity>.UpdateFailed());
			return false;
		}

		Notifier.Success(StatusMessages.Crud<TEntity>.UpdateSuccessful());
		return true;
	}

	protected async Task<bool> RunBeforeHooks(TEntity entity, bool isAdding)
	{
		try
		{
			var wasSuccessful = true;
			foreach (var validator in StateValidators)
			{
				if (!await validator.IsValid(entity, isAdding)) wasSuccessful = false;
			}

			if (!wasSuccessful) return false;

			foreach (var beforeHook in BeforeUpsertHooks)
			{
				await beforeHook.Handle(entity, isAdding);
			}
		}
		catch (Exception e)
		{
			Logger.LogError(
				e,
				"One or more before {action} hooks failed to run",
				isAdding ? "add" : "edit");
			return false;
		}

		return true;
	}

	protected async Task<bool> RunAfterHooks(TEntity entity, bool isAdding)
	{
		var successful = true;
		try
		{
			foreach (var afterHook in AfterUpsertHooks)
			{
				if (await afterHook.Handle(entity, isAdding) != HookStatus.Success) successful = false;
			}
		}
		catch (Exception e)
		{
			Logger.LogError(
				e,
				"One or more after {action} hooks failed to run",
				isAdding ? "add" : "edit");
			return false;
		}

		return successful;
	}
}

public class EntityWriter<TEntity> : EntityWriter<TEntity, DbContext>
	where TEntity : EntityBase, new()
{
	/// <inheritdoc />
	public EntityWriter(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<DbService<TEntity, DbContext>> logger,
		INotificationService notifier,
		IEnumerable<IEntityStateValidator<TEntity>> stateValidators,
		IEnumerable<IBeforeUpsert<TEntity>> beforeUpsertHooks,
		IEnumerable<IAfterUpsert<TEntity>> afterUpsertHooks)
		: base(
			contextAccessor,
			logger,
			notifier,
			stateValidators,
			beforeUpsertHooks,
			afterUpsertHooks) {}
}