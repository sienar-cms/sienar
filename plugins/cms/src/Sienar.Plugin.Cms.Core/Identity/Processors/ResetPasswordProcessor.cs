#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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

/// <exclude />
public class ResetPasswordProcessor : IProcessor<ResetPasswordRequest, bool>
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

	public async Task<HookResult<bool>> Process(ResetPasswordRequest request)
	{
		var user = await _userManager.GetSienarUser(request.UserId);
		if (user == null)
		{
			_notifier.Error(ErrorMessages.Account.AccountErrorInvalidId);
			return this.NotFound();
		}

		var status = await _vcManager.VerifyCode(
			user,
			VerificationCodeTypes.PasswordReset,
			request.VerificationCode,
			true);

		if (status == VerificationCodeStatus.Invalid)
		{
			_notifier.Error(ErrorMessages.Account.VerificationCodeInvalid);
			return this.NotFound();
		}

		if (status == VerificationCodeStatus.Expired)
		{
			if (_options.EnableEmail)
			{
				var sent = await _emailManager.SendPasswordResetEmail(user);
				_notifier.Error(ErrorMessages.Account.VerificationCodeExpired);
				if (!sent) _notifier.Error(ErrorMessages.Email.FailedToSend);
				return this.Unprocessable();
			}

			_notifier.Error(
				ErrorMessages.Account.VerificationCodeExpiredEmailDisabled);
			return this.Unprocessable();
		}

		// Code was valid
		await _userManager.UpdatePassword(user, request.NewPassword);

		return this.Success(true);
	}

	public void NotifySuccess()
	{
		_notifier.Success("Password reset successfully");
	}

	public void NotifyFailure()
	{
		_notifier.Error("An unknown error occurred while resetting your password");
	}

	public void NotifyNoPermission()
	{
		_notifier.Error("You do not have permission to reset your password");
	}
}