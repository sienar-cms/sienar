#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Identity.Requests;
using Sienar.Hooks;
using Sienar.Security;

namespace Sienar.Identity.Hooks;

/// <exclude />
public class RemoveUserRelatedEntitiesHook<TContext>
	: IBeforeAction<SienarUser>, IBeforeAction<DeleteAccountRequest>
	where TContext : DbContext
{
	private readonly TContext _context;
	private readonly IUserAccessor _userAccessor;

	public RemoveUserRelatedEntitiesHook(
		TContext context,
		IUserAccessor userAccessor)
	{
		_context = context;
		_userAccessor = userAccessor;
	}

	Task IBeforeAction<SienarUser>.Handle(
		SienarUser entity,
		ActionType action)
	{
		return action == ActionType.Delete
			? HandleCore(entity)
			: Task.CompletedTask;
	}

	async Task IBeforeAction<DeleteAccountRequest>.Handle(
		DeleteAccountRequest request,
		ActionType action)
	{
		if (action != ActionType.Status) return;

		var userId = (await _userAccessor.GetUserId())!;
		var user = (await _context
			.Set<SienarUser>()
			.FindAsync(userId.Value))!;
		await HandleCore(user);
	}

	private async Task HandleCore(SienarUser user)
	{
		await _context
			.Entry(user)
			.Collection(u => u.VerificationCodes)
			.LoadAsync();

		user.VerificationCodes.Clear();

		_context.Update(user);
		await _context.SaveChangesAsync();
	}
}