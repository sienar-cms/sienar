#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Data;
using Sienar.Identity.Data;
using Sienar.Processors;
using Sienar.Security;

namespace Sienar.Identity.Processors;

/// <exclude />
public class DeleteAccountProcessor : IStatusProcessor<DeleteAccountRequest>
{
	private readonly IUserAccessor _userAccessor;
	private readonly IUserRepository _userRepository;
	private readonly IPasswordManager _passwordManager;
	private readonly ISignInManager _signInManager;

	public DeleteAccountProcessor(
		IUserAccessor userAccessor,
		IUserRepository userRepository,
		IPasswordManager passwordManager,
		ISignInManager signInManager)
	{
		_userAccessor = userAccessor;
		_userRepository = userRepository;
		_passwordManager = passwordManager;
		_signInManager = signInManager;
	}

	public async Task<OperationResult<bool>> Process(DeleteAccountRequest request)
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

		if (!await _passwordManager.VerifyPassword(user, request.Password))
		{
			return new(
				OperationStatus.Unauthorized,
				message: CmsErrors.Account.LoginFailedInvalid);
		}

		var deleted = await _userRepository.Delete(user.Id);
		if (deleted)
		{
			await _signInManager.SignOut();
			return new(
				OperationStatus.Success,
				true,
				"Account deleted successfully");
		}

		return new(
			OperationStatus.Unknown,
			false,
			StatusMessages.Database.QueryFailed);
	}
}