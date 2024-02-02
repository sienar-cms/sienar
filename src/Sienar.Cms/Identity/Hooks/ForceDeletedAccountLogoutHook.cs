using System.Threading.Tasks;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Identity.Hooks;

public class ForceDeletedAccountLogoutHook : IAfterProcess<SienarUser>
{
	private readonly IForcedLogoutNotifier _forcedLogoutNotifier;

	public ForceDeletedAccountLogoutHook(IForcedLogoutNotifier forcedLogoutNotifier)
	{
		_forcedLogoutNotifier = forcedLogoutNotifier;
	}

	/// <inheritdoc />
	public Task Handle(SienarUser entity, ActionType action)
		=> action == ActionType.Delete
			? _forcedLogoutNotifier.ForceLogoutUser(entity.Id)
			: Task.CompletedTask;
}