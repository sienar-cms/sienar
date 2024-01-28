using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

public class ForgotPasswordProcessor : IProcessor<ForgotPasswordRequest>
{
	private readonly IUserManager _userManager;
	private readonly IVerificationCodeManager _vcManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly INotificationService _notifier;
	private readonly SienarOptions _options;

	public ForgotPasswordProcessor(
		IUserManager userManager,
		IVerificationCodeManager vcManager,
		IAccountEmailManager emailManager,
		INotificationService notifier,
		IOptions<SienarOptions> options)
	{
		_userManager = userManager;
		_vcManager = vcManager;
		_emailManager = emailManager;
		_notifier = notifier;
		_options = options.Value;
	}

	/// <inheritdoc />
	public async Task<HookStatus> Process(ForgotPasswordRequest request)
	{
		var user = await _userManager.GetSienarUser(request.AccountName);

		// They don't need to know whether the user exists or not
		// so if the user isn't null, send the email
		// otherwise, just return Ok anyway
		if (user != null && _options.EnableEmail)
		{
			await user.SendPasswordResetEmail(_vcManager, _emailManager);
		}

		return HookStatus.Success;
	}

	/// <inheritdoc />
	public void NotifySuccess()
	{
		_notifier.Success("Password reset requested");
	}

	/// <inheritdoc />
	public void NotifyBeforeHookFailure()
	{
		_notifier.Error("Unable to request password reset");
	}

	/// <inheritdoc />
	public void NotifyProcessFailure()
	{
		_notifier.Error("An unknown error occurred while requesting your password reset");
	}

	/// <inheritdoc />
	public void NotifyAfterHookFailure()
	{
		_notifier.Warning("Your password reset was requested successfully, but a third party plugin failed to execute");
	}
}