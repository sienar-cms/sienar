using System.Threading.Tasks;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Identity.Hooks;

public class LogoutHook : IProcessor<LogoutRequest>
{
	private readonly ISignInManager _signInManager;
	private readonly INotificationService _notifier;

	public LogoutHook(
		ISignInManager signInManager,
		INotificationService notifier)
	{
		_signInManager = signInManager;
		_notifier = notifier;
	}

	/// <inheritdoc />
	public async Task<HookStatus> Process(LogoutRequest request)
	{
		await _signInManager.SignOut();
		return HookStatus.Success;
	}

	/// <inheritdoc />
	public void NotifySuccess()
	{
		_notifier.Success("Logged out successfully");
	}

	/// <inheritdoc />
	public void NotifyBeforeHookFailure()
	{
		_notifier.Error("Unable to log out");
	}

	/// <inheritdoc />
	public void NotifyProcessFailure()
	{
		_notifier.Error("An unknown error occurred while logging out");
	}

	/// <inheritdoc />
	public void NotifyAfterHookFailure()
	{
		_notifier.Warning("You were logged out successfully, but a third party plugin failed to execute");
	}
}