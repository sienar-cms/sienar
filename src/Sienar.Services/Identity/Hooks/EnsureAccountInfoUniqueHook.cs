using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Errors;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Services;

namespace Sienar.Identity.Hooks;

public class EnsureAccountInfoUniqueHook : DbService<SienarUser>,
	IEntityStateValidator<SienarUser>,
	IBeforeProcess<RegisterRequest>
{
	/// <inheritdoc />
	public EnsureAccountInfoUniqueHook(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier)
		: base(contextAccessor, logger, notifier) {}

	/// <inheritdoc />
	Task<bool> IEntityStateValidator<SienarUser>.IsValid(SienarUser entity, bool adding)
		=> UserIsUnique(
			entity.Username,
			entity.Email,
			entity.PendingEmail,
			entity.Id);

	/// <inheritdoc />
	async Task<HookStatus> IBeforeProcess<RegisterRequest>.Handle(RegisterRequest request)
	{
		return await UserIsUnique(
			request.Username,
			request.Email)
			? HookStatus.Success
			: HookStatus.Conflict;
	}

	private async Task<bool> UserIsUnique(
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

		return valid;
	}
}