#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Identity.Hooks;

/// <exclude />
public class EnsureAccountInfoUniqueValidator : IStateValidator<SienarUser>,
	IStateValidator<RegisterRequest>
{
	private readonly DbContext _context;
	private readonly INotificationService _notifier;

	public EnsureAccountInfoUniqueValidator(
		DbContext context,
		INotificationService notifier)
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
		Guid id = default)
	{
		var valid = true;
		var entitySet = _context.Set<SienarUser>();

		var user = await entitySet.FirstOrDefaultAsync(u => u.Id != id && u.Username == username);
		if (user is not null)
		{
			_notifier.Error(CmsErrors.Account.UsernameTaken);
			valid = false;
		}

		if (!string.IsNullOrEmpty(pendingEmail))
		{
			user = await entitySet.FirstOrDefaultAsync(
				u => u.Id != id
				&& (u.Email == email
					|| u.Email == pendingEmail
					|| u.PendingEmail == email
					|| u.PendingEmail == pendingEmail));
		}
		else
		{
			user = await entitySet.FirstOrDefaultAsync(
				u => u.Id != id
				&& (u.Email == email
					|| u.PendingEmail == email));
		}

		if (user is not null)
		{
			_notifier.Error(CmsErrors.Account.EmailTaken);
			valid = false;
		}

		return valid
			? OperationStatus.Success
			: OperationStatus.Conflict;
	}
}