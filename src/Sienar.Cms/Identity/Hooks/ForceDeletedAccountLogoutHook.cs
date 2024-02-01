using System.Threading.Tasks;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Identity.Hooks;

public class ForceDeletedAccountLogoutHook : IAfterDelete<SienarUser>
{
	private readonly IForcedLogoutNotifier _forcedLogoutNotifier;

	public ForceDeletedAccountLogoutHook(IForcedLogoutNotifier forcedLogoutNotifier)
	{
		_forcedLogoutNotifier = forcedLogoutNotifier;
	}

	/// <inheritdoc />
	public Task<HookStatus> Handle(SienarUser entity)
	{
		_forcedLogoutNotifier.ForceLogoutUser(entity.Id);
		return Task.FromResult(HookStatus.Success);
	}
}