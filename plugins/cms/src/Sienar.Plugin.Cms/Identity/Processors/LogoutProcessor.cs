using System.Threading.Tasks;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

public class LogoutProcessor : IProcessor<LogoutRequest, bool>
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
	public async Task<HookResult<bool>> Process(LogoutRequest request)
	{
		await _signInManager.SignOut();
		return this.Success(true);
	}

	/// <inheritdoc />
	public void NotifySuccess()
	{
		_notifier.Success("Logged out successfully");
	}

	/// <inheritdoc />
	public void NotifyFailure()
	{
		_notifier.Error("An unknown error occurred while logging out");
	}

	/// <inheritdoc />
	public void NotifyNoPermission()
	{
		_notifier.Error("You do not have permission to log out");
	}
}