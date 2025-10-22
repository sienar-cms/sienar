#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Identity.Data;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ManuallyConfirmUserAccountProcessor
	: IStatusProcessor<ManuallyConfirmUserAccountRequest>
{
	private readonly IUserRepository _userRepository;

	public ManuallyConfirmUserAccountProcessor(
		IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<OperationResult<bool>> Process(ManuallyConfirmUserAccountRequest request)
	{
		var user = await _userRepository.Read(request.UserId);
		if (user is null)
		{
			return new(
				OperationStatus.NotFound,
				message: CmsErrors.Account.NotFound);
		}

		if (user.EmailConfirmed)
		{
			return new(
				OperationStatus.Unprocessable,
				message: $"{user.Username}'s account is already confirmed");
		}

		user.EmailConfirmed = true;

		return await _userRepository.Update(user)
			? new(
				OperationStatus.Success,
				true,
				$"Confirmed {user.Username}'s account")
			: new(
				OperationStatus.Unknown,
				message: StatusMessages.Database.QueryFailed);
	}
}