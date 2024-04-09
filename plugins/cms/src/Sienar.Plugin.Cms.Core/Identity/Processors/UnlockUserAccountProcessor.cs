#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Errors;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;
using Sienar.Infrastructure.Services;

namespace Sienar.Identity.Processors;

/// <exclude />
public class UnlockUserAccountProcessor : DbService<SienarUser>,
	IProcessor<UnlockUserAccountRequest, bool>
{
	private readonly IUserManager _userManager;
	private SienarUser? _user;

	public UnlockUserAccountProcessor(
		DbContext context,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager)
		: base(context, logger, notifier)
	{
		_userManager = userManager;
	}

	public async Task<HookResult<bool>> Process(UnlockUserAccountRequest request)
	{
		_user = await _userManager.GetSienarUser(
			request.UserId,
			u => u.LockoutReasons);
		if (_user is null)
		{
			Notifier.Error(ErrorMessages.Account.NotFound);
			return this.NotFound();
		}

		_user.LockoutEnd = null;
		_user.LockoutReasons.Clear();

		EntitySet.Update(_user);
		await Context.SaveChangesAsync();

		return this.Success(true);
	}

	public void NotifySuccess()
	{
		Notifier.Success($"User {_user?.Username}'s account was unlocked successfully");
	}

	public void NotifyFailure()
	{
		Notifier.Error("Failed to unlock user account");
	}

	public void NotifyNoPermission()
	{
		Notifier.Error("You do not have permission to unlock user accounts");
	}
}