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
	private readonly IEnumerable<IEntityStateValidator<TEntity>> _stateValidators;
	private readonly IEnumerable<IBeforeUpsert<TEntity>> _beforeUpsertHooks;
	private readonly IEnumerable<IAfterUpsert<TEntity>> _afterUpsertHooks;

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
		_stateValidators = stateValidators;
		_beforeUpsertHooks = beforeUpsertHooks;
		_afterUpsertHooks = afterUpsertHooks;
	}

	/// <inheritdoc />
	public async Task<Guid> Add(TEntity model)
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

		await RunAfterHooks(model, true);
		Notifier.Success(StatusMessages.Crud<TEntity>.CreateSuccessful());
		return model.Id;
	}

	/// <inheritdoc />
	public async Task<bool> Edit(TEntity model)
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

		await RunAfterHooks(model, false);
		Notifier.Success(StatusMessages.Crud<TEntity>.UpdateSuccessful());
		return true;
	}

	private async Task<bool> RunBeforeHooks(TEntity entity, bool isAdding)
	{
		try
		{
			var wasSuccessful = true;
			foreach (var validator in _stateValidators)
			{
				if (!await validator.IsValid(entity, isAdding)) wasSuccessful = false;
			}

			if (!wasSuccessful) return false;

			foreach (var beforeHook in _beforeUpsertHooks)
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

	private async Task RunAfterHooks(TEntity entity, bool isAdding)
	{
		foreach (var afterHook in _afterUpsertHooks)
		{
			try
			{
				await afterHook.Handle(entity, isAdding);
			}
			catch (Exception e)
			{
				Logger.LogError(
					e,
					"One or more after {action} hooks failed to run",
					isAdding ? "add" : "edit");
			}
		}
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