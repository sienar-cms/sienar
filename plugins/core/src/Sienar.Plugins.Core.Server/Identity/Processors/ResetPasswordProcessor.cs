#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Email;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Identity.Data;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ResetPasswordProcessor : IStatusProcessor<ResetPasswordRequest>
{
	private readonly IUserRepository _userRepository;
	private readonly IPasswordManager _passwordManager;
	private readonly IVerificationCodeManager _vcManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly SienarOptions _options;

	public ResetPasswordProcessor(
		IUserRepository userRepository,
		IPasswordManager passwordManager,
		IVerificationCodeManager vcManager,
		IAccountEmailManager emailManager,
		IOptions<SienarOptions> options)
	{
		_userRepository = userRepository;
		_passwordManager = passwordManager;
		_vcManager = vcManager;
		_emailManager = emailManager;
		_options = options.Value;
	}

	public async Task<OperationResult<bool>> Process(ResetPasswordRequest request)
	{
		var user = await _userRepository.Read(request.UserId);
		if (user == null)
		{
			return new(
				OperationStatus.NotFound,
				message: CmsErrors.Account.AccountErrorInvalidId);
		}

		var status = await _vcManager.VerifyCode(
			user,
			VerificationCodeTypes.PasswordReset,
			request.VerificationCode,
			true);

		if (status == VerificationCodeStatus.Invalid)
		{
			return new(
				OperationStatus.NotFound,
				message: CmsErrors.Account.VerificationCodeInvalid);
		}

		if (status == VerificationCodeStatus.Expired)
		{
			if (_options.EnableEmail)
			{
				await _emailManager.SendPasswordResetEmail(user);
				return new(
					OperationStatus.Unprocessable,
					message: CmsErrors.Account.VerificationCodeExpired);
			}

			return new(
				OperationStatus.Unprocessable,
                message: CmsErrors.Account.VerificationCodeExpiredEmailDisabled);
		}

		// Code was valid
		await _passwordManager.UpdatePassword(user, request.NewPassword);

		return new(
			OperationStatus.Success,
			true,
			"Password reset successfully");
	}
}