#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Extensions;
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

		user.NormalizedUsername = user.Username.ToNormalized();
		user.NormalizedEmail = user.Email.ToNormalized();
		user.NormalizedPendingEmail = user.PendingEmail?.ToNormalized();
		return Task.CompletedTask;
	}
}