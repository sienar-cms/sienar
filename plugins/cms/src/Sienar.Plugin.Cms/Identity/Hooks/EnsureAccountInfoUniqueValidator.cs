using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Services;

namespace Sienar.Identity.Hooks;

public class EnsureAccountInfoUniqueValidator : DbService<SienarUser>,
	IStateValidator<SienarUser>,
	IStateValidator<RegisterRequest>
{
	/// <inheritdoc />
	public EnsureAccountInfoUniqueValidator(
		DbContext context,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier)
		: base(context, logger, notifier) {}

	/// <inheritdoc />
	Task<HookStatus> IStateValidator<SienarUser>.Validate(SienarUser entity, ActionType type)
		=> UserIsUnique(
			entity.Username,
			entity.Email,
			entity.PendingEmail,
			entity.Id);

	/// <inheritdoc />
	Task<HookStatus> IStateValidator<RegisterRequest>.Validate(
		RegisterRequest request,
		ActionType action)
		=> UserIsUnique(
			request.Username,
			request.Email);

	private async Task<HookStatus> UserIsUnique(
		string username,
		string email,
		string? pendingEmail = null,
		Guid id = default)
	{
		var valid = true;

		var user = await EntitySet.FirstOrDefaultAsync(
			u => u.Id != id && u.Username == username);
		if (user is not null)
		{
			Notifier.Error(ErrorMessages.Account.UsernameTaken);
			valid = false;
		}

		if (!string.IsNullOrEmpty(pendingEmail))
		{
			user = await EntitySet.FirstOrDefaultAsync(
				u => u.Id != id
				&& (u.Email == email
					|| u.Email == pendingEmail
					|| u.PendingEmail == email
					|| u.PendingEmail == pendingEmail));
		}
		else
		{
			user = await EntitySet.FirstOrDefaultAsync(
				u => u.Id != id
				&& (u.Email == email
					|| u.PendingEmail == email));
		}

		if (user is not null)
		{
			Notifier.Error(ErrorMessages.Account.EmailTaken);
			valid = false;
		}

		return valid
			? HookStatus.Success
			: HookStatus.Conflict;
	}
}