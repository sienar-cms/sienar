﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Services;

namespace Sienar.Infrastructure.Hooks;

public class ConcurrencyStampValidator<TEntity>
	: DbService<TEntity, DbContext>, IStateValidator<TEntity>
	where TEntity : EntityBase
{
	/// <inheritdoc />
	public ConcurrencyStampValidator(
		DbContext context,
		ILogger<DbService<TEntity, DbContext>> logger,
		INotificationService notifier)
		: base(context, logger, notifier) {}

	/// <inheritdoc />
	public async Task<HookStatus> Validate(TEntity entity, ActionType action)
	{
		// Only run on update
		if (action is not ActionType.Update) return HookStatus.Success;

		var concurrencyStamp = await EntitySet
			.Where(m => m.Id == entity.Id)
			.Select(m => m.ConcurrencyStamp)
			.FirstOrDefaultAsync();

		if (concurrencyStamp == Guid.Empty
			|| concurrencyStamp != entity.ConcurrencyStamp)
		{
			Notifier.Error($"Unable to update {typeof(TEntity).Name}: the entity has been updated by another user.");
			return HookStatus.Conflict;
		}

		return HookStatus.Success;
	}
}