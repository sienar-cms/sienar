#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Data;
using Sienar.Infrastructure;
using Sienar.Hooks;
using Sienar.Identity.Data;

namespace Sienar.Identity.Hooks;

/// <exclude />
public class EnsureAccountInfoUniqueValidator : IStateValidator<SienarUser>,
	IStateValidator<RegisterRequest>
{
	private readonly IUserRepository _userRepository;
	private readonly INotifier _notifier;

	public EnsureAccountInfoUniqueValidator(
		IUserRepository userRepository,
		INotifier notifier)
	{
		_userRepository = userRepository;
		_notifier = notifier;
	}

	Task<OperationStatus> IStateValidator<SienarUser>.Validate(SienarUser request, ActionType type)
		=> UserIsUnique(
			request.Username,
			request.Email,
			request.PendingEmail,
			request.Id);

	Task<OperationStatus> IStateValidator<RegisterRequest>.Validate(
		RegisterRequest request,
		ActionType action)
		=> UserIsUnique(
			request.Username,
			request.Email);

	private async Task<OperationStatus> UserIsUnique(
		string username,
		string email,
		string? pendingEmail = null,
		int id = 0)
	{
		if (await _userRepository.UsernameIsTaken(id, username))
		{
			_notifier.Error(CmsErrors.Account.UsernameTaken);
			return OperationStatus.Conflict;
		}

		if (await _userRepository.EmailIsTaken(id, email) ||
			(!string.IsNullOrEmpty(pendingEmail) && await _userRepository.EmailIsTaken(id, pendingEmail)))
		{
			_notifier.Error(CmsErrors.Account.EmailTaken);
			return OperationStatus.Conflict;
		}

		return OperationStatus.Success;
	}
}