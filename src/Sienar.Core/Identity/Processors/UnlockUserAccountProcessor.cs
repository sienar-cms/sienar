using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;
using Sienar.Infrastructure.Services;

namespace Sienar.Identity.Processors;

public class UnlockUserAccountProcessor : DbService<SienarUser>,
	IProcessor<UnlockUserAccountRequest>
{
	private readonly IUserManager _userManager;
	private SienarUser? _user;

	/// <inheritdoc />
	public UnlockUserAccountProcessor(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager)
		: base(contextAccessor, logger, notifier)
	{
		_userManager = userManager;
	}

	/// <inheritdoc />
	public async Task<HookStatus> Process(UnlockUserAccountRequest request)
	{
		_user = await _userManager.GetSienarUser(
			request.UserId,
			u => u.LockoutReasons);
		if (_user is null)
		{
			Notifier.Error(ErrorMessages.Account.NotFound);
			return HookStatus.NotFound;
		}

		_user.LockoutEnd = null;
		_user.LockoutReasons.Clear();

		EntitySet.Update(_user);
		await Context.SaveChangesAsync();

		return HookStatus.Success;
	}

	/// <inheritdoc />
	public void NotifySuccess()
	{
		Notifier.Success($"User {_user?.Username}'s account was unlocked successfully");
	}

	/// <inheritdoc />
	public void NotifyFailure()
	{
		Notifier.Error("Failed to unlock user account");
	}

	/// <inheritdoc />
	public void NotifyNoPermission()
	{
		Notifier.Error("You do not have permission to unlock user accounts");
	}
}