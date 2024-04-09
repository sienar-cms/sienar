#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
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

	public async Task<HookResult<bool>> Process(LogoutRequest request)
	{
		await _signInManager.SignOut();
		return this.Success(true);
	}

	public void NotifySuccess()
	{
		_notifier.Success("Logged out successfully");
	}

	public void NotifyFailure()
	{
		_notifier.Error("An unknown error occurred while logging out");
	}

	public void NotifyNoPermission()
	{
		_notifier.Error("You do not have permission to log out");
	}
}