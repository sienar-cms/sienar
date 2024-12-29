#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Data;
using Sienar.Processors;

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

	public async Task<OperationResult<bool>> Process(LogoutRequest request)
	{
		await _signInManager.SignOut();
		return new(
			OperationStatus.Success,
			true,
			"Logged out successfully");
	}
}