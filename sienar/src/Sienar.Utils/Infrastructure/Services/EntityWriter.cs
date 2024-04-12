#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Extensions;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Infrastructure.Services;

/// <exclude />
public class EntityWriter<TEntity, TContext> : DbService<TEntity, TContext>, IEntityWriter<TEntity>
	where TEntity : EntityBase, new()
	where TContext : DbContext
{
	private readonly IEnumerable<IAccessValidator<TEntity>> _accessValidators;
	private readonly IEnumerable<IStateValidator<TEntity>> _stateValidators;
	private readonly IEnumerable<IBeforeProcess<TEntity>> _beforeHooks;
	private readonly IEnumerable<IAfterProcess<TEntity>> _afterHooks;

	public EntityWriter(
		TContext context,
		ILogger<DbService<TEntity, TContext>> logger,
		INotificationService notifier,
		IEnumerable<IAccessValidator<TEntity>> accessValidators,
		IEnumerable<IStateValidator<TEntity>> stateValidators,
		IEnumerable<IBeforeProcess<TEntity>> beforeHooks,
		IEnumerable<IAfterProcess<TEntity>> afterHooks)
		: base(context, logger, notifier)
	{
		_accessValidators = accessValidators;
		_stateValidators = stateValidators;
		_beforeHooks = beforeHooks;
		_afterHooks = afterHooks;
	}

	public async Task<Guid> Create(TEntity model)
	{
		if (!await _accessValidators.Validate(model, ActionType.Create, Logger))
		{
			Notifier.Error(StatusMessages.Crud<TEntity>.NoPermission());
			return Guid.Empty;
		}

		if (!await _stateValidators.Validate(model, ActionType.Create, Logger)
		|| !await _beforeHooks.Run(model, ActionType.Create, Logger))
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

		await _afterHooks.Run(model, ActionType.Create, Logger);
		Notifier.Success(StatusMessages.Crud<TEntity>.CreateSuccessful());
		return model.Id;
	}

	public async Task<bool> Update(TEntity model)
	{
		if (!await _accessValidators.Validate(model, ActionType.Update, Logger))
		{
			Notifier.Error(StatusMessages.Crud<TEntity>.NoPermission());
			return false;
		}

		if (!await _stateValidators.Validate(model, ActionType.Update, Logger)
		|| !await _beforeHooks.Run(model, ActionType.Update, Logger))
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

		await _afterHooks.Run(model, ActionType.Update, Logger);
		Notifier.Success(StatusMessages.Crud<TEntity>.UpdateSuccessful());
		return true;
	}
}

/// <exclude />
public class EntityWriter<TEntity> : EntityWriter<TEntity, DbContext>
	where TEntity : EntityBase, new()
{
	public EntityWriter(
		DbContext context,
		ILogger<DbService<TEntity, DbContext>> logger,
		INotificationService notifier,
		IEnumerable<IAccessValidator<TEntity>> accessValidators,
		IEnumerable<IStateValidator<TEntity>> stateValidators,
		IEnumerable<IBeforeProcess<TEntity>> beforeHooks,
		IEnumerable<IAfterProcess<TEntity>> afterHooks)
		: base(
			context,
			logger,
			notifier,
			accessValidators,
			stateValidators,
			beforeHooks,
			afterHooks) {}
}