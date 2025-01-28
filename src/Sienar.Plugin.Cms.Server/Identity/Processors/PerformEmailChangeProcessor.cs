#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Email;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Data;
using Sienar.Identity.Data;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class PerformEmailChangeProcessor
	: IStatusProcessor<PerformEmailChangeRequest>
{
	private readonly IUserRepository _userRepository;
	private readonly IUserAccessor _userAccessor;
	private readonly IVerificationCodeManager _vcManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly SienarOptions _sienarOptions;

	public PerformEmailChangeProcessor(
		IUserRepository userRepository,
		IUserAccessor userAccessor,
		IVerificationCodeManager vcManager,
		IAccountEmailManager emailManager,
		IOptions<SienarOptions> sienarOptions)
	{
		_userRepository = userRepository;
		_userAccessor = userAccessor;
		_vcManager = vcManager;
		_emailManager = emailManager;
		_sienarOptions = sienarOptions.Value;
	}

	public async Task<OperationResult<bool>> Process(PerformEmailChangeRequest request)
	{
		var userId = await _userAccessor.GetUserId();
		if (!userId.HasValue)
		{
			return new(
				OperationStatus.Unauthorized,
				message: CmsErrors.Account.LoginRequired);
		}

		var user = await _userRepository.Read(userId.Value);
		if (user is null)
		{
			return new(
				OperationStatus.Unauthorized,
				message: CmsErrors.Account.LoginRequired);
		}

		if (user.Id != request.UserId)
		{
			return new(
				OperationStatus.Unprocessable,
				message: CmsErrors.Account.AccountErrorWrongId);
		}

		if (string.IsNullOrEmpty(user.PendingEmail))
		{
			return new(
				OperationStatus.Unprocessable,
				message: CmsErrors.Account.NoPendingEmail);
		}

		var status = await _vcManager.VerifyCode(
			user,
			VerificationCodeTypes.ChangeEmail,
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
			if (_sienarOptions.EnableEmail)
			{
				await _emailManager.SendEmailChangeConfirmationEmail(user);
				return new(
					OperationStatus.Unprocessable,
					message: CmsErrors.Account.VerificationCodeExpired);
			}

			return new(
				OperationStatus.Unprocessable,
				message: CmsErrors.Account.VerificationCodeExpiredEmailDisabled);
		}

		// Code was valid
		user.Email = user.PendingEmail;
		user.NormalizedEmail = user.NormalizedPendingEmail!.ToUpperInvariant();
		user.PendingEmail = null;
		user.NormalizedPendingEmail = null;

		return await _userRepository.Update(user)
			? new(
				OperationStatus.Success,
				true,
				"Email changed successfully")
			: new(
				OperationStatus.Unknown,
				message: StatusMessages.Database.QueryFailed);
	}
}