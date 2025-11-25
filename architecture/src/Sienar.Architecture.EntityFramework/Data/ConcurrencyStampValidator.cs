#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Hooks;
using Sienar.Infrastructure;

namespace Sienar.Data;

/// <exclude />
public class ConcurrencyStampValidator<TEntity, TContext> : IStateValidator<TEntity>
	where TEntity : EntityBase
	where TContext : DbContext
{
	private readonly TContext _context;
	private readonly INotifier _notifier;

	public ConcurrencyStampValidator(
		TContext context,
		INotifier notifier)
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
			.Where(e => e.Id == request.Id)
			.Select(e => e.ConcurrencyStamp)
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