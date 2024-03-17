using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Email;
using Sienar.Errors;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

public class ForgotPasswordProcessor : IProcessor<ForgotPasswordRequest, bool>
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
	public async Task<HookResult<bool>> Process(ForgotPasswordRequest request)
	{
		var user = await _userManager.GetSienarUser(request.AccountName);

		// If the user doesn't exist, they don't need to know
		// Just return success
		if (user is null) return this.Success(true);

		if (user.IsLockedOut())
		{
			_notifier.Error(ErrorMessages.Account.AccountLocked);
			return this.Unauthorized();
		}

		// They don't need to know whether the user exists or not
		// so if the user isn't null, send the email
		// otherwise, just return Ok anyway
		if (_options.EnableEmail)
		{
			await _emailManager.SendPasswordResetEmail(_vcManager, user);
		}

		return this.Success(true);
	}

	/// <inheritdoc />
	public void NotifySuccess()
	{
		_notifier.Success("Password reset requested");
	}

	/// <inheritdoc />
	public void NotifyFailure()
	{
		_notifier.Error("An unknown error occurred while requesting your password reset");
	}

	/// <inheritdoc />
	public void NotifyNoPermission()
	{
		_notifier.Error("You do not have permission to reset your password");
	}
}