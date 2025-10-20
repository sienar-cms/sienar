#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Errors;
using Sienar.Identity.Data;
using Sienar.Identity.Requests;
using Sienar.Identity.Results;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class GetLockoutReasonsProcessor
	: IProcessor<AccountLockoutRequest, AccountLockoutResult>
{
	private readonly IUserRepository _userRepo;
	private readonly IVerificationCodeManager _vcManager;

	public GetLockoutReasonsProcessor(
		IUserRepository userRepo,
		IVerificationCodeManager vcManager)
	{
		_userRepo = userRepo;
		_vcManager = vcManager;
	}

	public async Task<OperationResult<AccountLockoutResult?>> Process(
		AccountLockoutRequest request)
	{
		var user = await _userRepo.Read(
			request.UserId,
			Filter.WithIncludes("VerificationCodes", "LockoutReasons"));
		if (user is null)
		{
			return new(
				OperationStatus.NotFound,
				message: StatusMessages.Crud<SienarUser>.NotFound(request.UserId));
		}

		var status = await _vcManager.VerifyCode(
			user,
			VerificationCodeTypes.ViewLockoutReasons,
			request.VerificationCode,
			true);

		if (status is VerificationCodeStatus.Invalid)
		{
			return new(
				OperationStatus.NotFound,
				message: CmsErrors.Account.VerificationCodeInvalid);
		}

		if (status is VerificationCodeStatus.Expired)
		{
			return new(
				OperationStatus.Unprocessable,
				message: CmsErrors.Account.VerificationCodeExpired);
		}

		if (user.LockoutReasons.Count == 0)
		{
			user.LockoutReasons.Add(new()
			{
				Reason = "You have attempted to log in with invalid credentials too many times"
			});
		}

		return new(
			OperationStatus.Success,
			new()
			{
				LockoutReasons = user.LockoutReasons,
				LockoutEnd = user.LockoutEnd == DateTime.MaxValue ? null : user.LockoutEnd
			});
	}
}