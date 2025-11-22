#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading.Tasks;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Data;
using Sienar.Email;
using Sienar.Identity.Data;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class LockUserAccountProcessor : IStatusProcessor<LockUserAccountRequest>
{
	private readonly IUserRepository _userRepository;
	private readonly ILockoutReasonRepository _lockoutReasonRepository;
	private readonly IAccountEmailManager _emailManager;

	public LockUserAccountProcessor(
		IUserRepository userRepository,
		ILockoutReasonRepository lockoutReasonRepository,
		IAccountEmailManager emailManager)
	{
		_userRepository = userRepository;
		_lockoutReasonRepository = lockoutReasonRepository;
		_emailManager = emailManager;
	}

	public async Task<OperationResult<bool>> Process(LockUserAccountRequest request)
	{
		var user = await _userRepository.Read(
			request.UserId,
			Filter.WithIncludes(nameof(SienarUser.LockoutReasons)));

		if (user is null)
		{
			return new(
				OperationStatus.NotFound,
				message: CoreErrors.Account.NotFound);
		}

		var reasons = await _lockoutReasonRepository.Read(request.Reasons);
		if (reasons.Count != request.Reasons.Count)
		{
			return new(
				OperationStatus.NotFound,
				message: CoreErrors.LockoutReason.NotFound);
		}

		user.LockoutReasons.AddRange(reasons);
		user.LockoutEnd = request.EndDate ?? DateTime.MaxValue;

		if (!await _userRepository.Update(user))
		{
			return new(
				OperationStatus.Unknown,
				message: StatusMessages.Database.QueryFailed);
		}

		if (!await _emailManager.SendAccountLockedEmail(user))
		{
			return new(
				OperationStatus.Success,
				true,
				$"User {user.Username} was locked successfully, but the email notification failed to send.");
		}

		return new(
			OperationStatus.Success,
			true,
			$"Locked user {user.Username} successfully");
	}
}