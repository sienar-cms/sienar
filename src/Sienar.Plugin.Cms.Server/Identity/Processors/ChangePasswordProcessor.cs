#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Data;
using Sienar.Identity.Data;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ChangePasswordProcessor : IProcessor<ChangePasswordRequest, bool>
{
	private readonly IUserRepository _userRepository;
	private readonly IUserAccessor _userAccessor;
	private readonly IPasswordManager _passwordManager;

	public ChangePasswordProcessor(
		IUserRepository userRepository,
		IUserAccessor userAccessor,
		IPasswordManager passwordManager)
	{
		_userRepository = userRepository;
		_userAccessor = userAccessor;
		_passwordManager = passwordManager;
	}

	public async Task<OperationResult<bool>> Process(ChangePasswordRequest request)
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

		if (!await _passwordManager.VerifyPassword(user, request.CurrentPassword))
		{
			return new(
				OperationStatus.Unauthorized,
				message: CmsErrors.Account.LoginFailedInvalid);
		}

		await _passwordManager.UpdatePassword(user, request.NewPassword);

		return new(
			OperationStatus.Success,
			true,
			"Password changed successfully");
	}
}