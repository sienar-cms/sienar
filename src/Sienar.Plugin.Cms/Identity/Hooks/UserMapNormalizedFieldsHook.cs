#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Hooks;

namespace Sienar.Identity.Hooks;

/// <exclude />
public class UserMapNormalizedFieldsHook : IBeforeAction<SienarUser>
{
	public Task Handle(SienarUser user, ActionType action)
	{
		if (action is not (ActionType.Create or ActionType.Update))
		{
			return Task.CompletedTask;
		}

		user.NormalizedUsername = user.Username.ToUpperInvariant();
		user.NormalizedEmail = user.Email.ToUpperInvariant();
		user.NormalizedPendingEmail = user.PendingEmail?.ToUpperInvariant();
		return Task.CompletedTask;
	}
}