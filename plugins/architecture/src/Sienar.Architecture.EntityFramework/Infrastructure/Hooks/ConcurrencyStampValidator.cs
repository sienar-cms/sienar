﻿#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Infrastructure.Data;

namespace Sienar.Infrastructure.Hooks;

/// <exclude />
public class ConcurrencyStampValidator<TEntity> : IStateValidator<TEntity>
	where TEntity : EntityBase
{
	private readonly DbContext _context;
	private readonly INotificationService _notifier;

	public ConcurrencyStampValidator(
		DbContext context,
		INotificationService notifier)
	{
		_context = context;
		_notifier = notifier;
	}

	public async Task<OperationStatus> Validate(TEntity request, ActionType action)
	{
		// Only run on update
		if (action is not ActionType.Update) return OperationStatus.Success;

		var concurrencyStamp = await _context
			.Set<TEntity>()
			.Where(m => m.Id == request.Id)
			.Select(m => m.ConcurrencyStamp)
			.FirstOrDefaultAsync();

		if (concurrencyStamp == Guid.Empty
			|| concurrencyStamp != request.ConcurrencyStamp)
		{
			_notifier.Error(
				$"Unable to update {typeof(TEntity).Name}: the entity has been updated by another user.");
			return OperationStatus.Conflict;
		}

		return OperationStatus.Success;
	}
}