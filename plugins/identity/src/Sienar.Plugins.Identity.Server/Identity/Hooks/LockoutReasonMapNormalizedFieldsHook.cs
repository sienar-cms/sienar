#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Hooks;

namespace Sienar.Identity.Hooks;

/// <exclude />
public class LockoutReasonMapNormalizedFieldsHook : IBeforeAction<LockoutReason>
{
	public Task Handle(LockoutReason request, ActionType action)
	{
		if (action is not (ActionType.Create or ActionType.Update))
		{
			return Task.CompletedTask;
		}

		request.NormalizedReason = request.Reason.ToUpperInvariant();
		return Task.CompletedTask;
	}
}