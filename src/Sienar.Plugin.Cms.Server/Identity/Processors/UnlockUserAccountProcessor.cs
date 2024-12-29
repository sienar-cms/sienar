#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Data;
using Sienar.Identity.Data;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class UnlockUserAccountProcessor : IProcessor<UnlockUserAccountRequest, bool>
{
	private readonly IUserRepository _userRepository;

	public UnlockUserAccountProcessor(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<OperationResult<bool>> Process(UnlockUserAccountRequest request)
	{
		var user = await _userRepository.Read(
			request.UserId,
			Filter.WithIncludes(nameof(SienarUser.LockoutReasons)));
		if (user is null)
		{
			return new(
				OperationStatus.NotFound,
				message: CmsErrors.Account.NotFound);
		}

		user.LockoutEnd = null;
		user.LockoutReasons.Clear();

		return await _userRepository.Update(user)
			? new(
				OperationStatus.Success,
				true,
				$"User {user.Username}'s account was unlocked successfully")
			: new(
				OperationStatus.Unknown,
				message: StatusMessages.Database.QueryFailed);
	}
}