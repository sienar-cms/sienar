using System.Threading.Tasks;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

public class LogoutProcessor : IProcessor<LogoutRequest>
{
	private readonly ISignInManager _signInManager;
	private readonly INotificationService _notifier;

	public LogoutProcessor(
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
	public void NotifyProcessFailure()
	{
		_notifier.Error("An unknown error occurred while logging out");
	}
}