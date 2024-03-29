﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Extensions;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Infrastructure.Services;

public class EntityDeleter<TEntity, TContext>
	: DbService<TEntity, TContext>, IEntityDeleter<TEntity>
	where TEntity : EntityBase
	where TContext : DbContext
{
	private readonly IEnumerable<IAccessValidator<TEntity>> _accessValidators;
	private readonly IEnumerable<IStateValidator<TEntity>> _stateValidators;
	private readonly IEnumerable<IBeforeProcess<TEntity>> _beforeHooks;
	private readonly IEnumerable<IAfterProcess<TEntity>> _afterHooks;

	/// <inheritdoc />
	public EntityDeleter(
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

		if (!await _accessValidators.Validate(entity, ActionType.Delete, Logger))
		{
			Notifier.Error(StatusMessages.Crud<TEntity>.NoPermission());
			return false;
		}

		if (!await _stateValidators.Validate(entity, ActionType.Delete, Logger)
			|| !await _beforeHooks.Run(entity, ActionType.Delete, Logger))
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

		await _afterHooks.Run(entity, ActionType.Delete, Logger);
		Notifier.Success(StatusMessages.Crud<TEntity>.DeleteSuccessful());
		return true;
	}
}

public class EntityDeleter<TEntity> : EntityDeleter<TEntity, DbContext>
	where TEntity : EntityBase
{
	/// <inheritdoc />
	public EntityDeleter(
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