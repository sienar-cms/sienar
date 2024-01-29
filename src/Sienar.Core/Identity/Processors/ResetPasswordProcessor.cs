using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Email;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

public class ResetPasswordProcessor : IProcessor<ResetPasswordRequest>
{
	private readonly IUserManager _userManager;
	private readonly IVerificationCodeManager _vcManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly INotificationService _notifier;
	private readonly SienarOptions _options;

	public ResetPasswordProcessor(
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
	public async Task<HookStatus> Process(ResetPasswordRequest request)
	{
		var user = await _userManager.GetSienarUser(request.UserId);
		if (user == null)
		{
			_notifier.Error(ErrorMessages.Account.AccountErrorInvalidId);
			return HookStatus.NotFound;
		}

		var status = await _vcManager.VerifyCode(
			user,
			VerificationCodeTypes.PasswordReset,
			request.VerificationCode,
			true);

		if (status == VerificationCodeStatus.Invalid)
		{
			_notifier.Error(ErrorMessages.Account.VerificationCodeInvalid);
			return HookStatus.NotFound;
		}

		if (status == VerificationCodeStatus.Expired)
		{
			if (_options.EnableEmail)
			{
				await user.SendPasswordResetEmail(_vcManager, _emailManager);
				_notifier.Error(ErrorMessages.Account.VerificationCodeExpired);
				return HookStatus.Unprocessable;
			}

			_notifier.Error(
				ErrorMessages.Account.VerificationCodeExpiredEmailDisabled);
			return HookStatus.Unprocessable;
		}

		// Code was valid
		await _userManager.UpdatePassword(user, request.NewPassword);

		return HookStatus.Success;
	}

	/// <inheritdoc />
	public void NotifySuccess()
	{
		_notifier.Success("Password reset successfully");
	}

	/// <inheritdoc />
	public void NotifyProcessFailure()
	{
		_notifier.Error("An unknown error occurred while resetting your password");
	}
}