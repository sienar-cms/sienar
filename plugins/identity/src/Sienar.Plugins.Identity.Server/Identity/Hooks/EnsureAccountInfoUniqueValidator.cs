#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Data;
using Sienar.Extensions;
using Sienar.Infrastructure;
using Sienar.Hooks;
using Sienar.Identity.Data;

namespace Sienar.Identity.Hooks;

/// <exclude />
public class EnsureAccountInfoUniqueValidator<TContext>
	: IStateValidator<SienarUser>, IStateValidator<RegisterRequest>
	where TContext : DbContext
{
	private readonly TContext _context;
	private readonly INotifier _notifier;

	public EnsureAccountInfoUniqueValidator(
		TContext context,
		INotifier notifier)
	{
		_context = context;
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
		var userSet = _context.Set<SienarUser>();

		username = username.ToNormalized();
		var usernameTaken = await userSet
			.AnyAsync(u => u.Id != id &&
				u.NormalizedUsername == username);
		if (usernameTaken)
		{
			_notifier.Error(CoreErrors.Account.UsernameTaken);
			return OperationStatus.Conflict;
		}

		email = email.ToNormalized();
		var emailTaken = await userSet
			.AnyAsync(u => u.Id != id &&
				(u.NormalizedEmail == email || u.NormalizedPendingEmail == email));
		if (emailTaken)
		{
			_notifier.Error(CoreErrors.Account.EmailTaken);
			return OperationStatus.Conflict;
		}

		return OperationStatus.Success;
	}
}