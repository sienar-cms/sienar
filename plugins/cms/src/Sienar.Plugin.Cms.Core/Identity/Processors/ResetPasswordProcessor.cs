#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Email;
using Sienar.Errors;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ResetPasswordProcessor : IProcessor<ResetPasswordRequest, bool>
{
	private readonly IUserManager _userManager;
	private readonly IVerificationCodeManager _vcManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly SienarOptions _options;

	public ResetPasswordProcessor(
		IUserManager userManager,
		IVerificationCodeManager vcManager,
		IAccountEmailManager emailManager,
		IOptions<SienarOptions> options)
	{
		_userManager = userManager;
		_vcManager = vcManager;
		_emailManager = emailManager;
		_options = options.Value;
	}

	public async Task<OperationResult<bool>> Process(ResetPasswordRequest request)
	{
		var user = await _userManager.GetSienarUser(request.UserId);
		if (user == null)
		{
			return this.NotFound(message: CmsErrors.Account.AccountErrorInvalidId);
		}

		var status = await _vcManager.VerifyCode(
			user,
			VerificationCodeTypes.PasswordReset,
			request.VerificationCode,
			true);

		if (status == VerificationCodeStatus.Invalid)
		{
			return this.NotFound(message: CmsErrors.Account.VerificationCodeInvalid);
		}

		if (status == VerificationCodeStatus.Expired)
		{
			if (_options.EnableEmail)
			{
				await _emailManager.SendPasswordResetEmail(user);
				return this.Unprocessable(message: CmsErrors.Account.VerificationCodeExpired);
			}

			return this.Unprocessable(message: CmsErrors.Account.VerificationCodeExpiredEmailDisabled);
		}

		// Code was valid
		await _userManager.UpdatePassword(user, request.NewPassword);

		return this.Success(true, "Password reset successfully");
	}
}