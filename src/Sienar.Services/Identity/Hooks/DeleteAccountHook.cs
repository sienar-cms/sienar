using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Errors;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Services;

namespace Sienar.Identity.Hooks;

public class DeleteAccountHook : DbService<SienarUser>,
	IProcessor<DeleteAccountRequest>
{
	private readonly IUserAccessor _userAccessor;
	private readonly IUserManager _userManager;
	private readonly ISignInManager _signInManager;

	/// <inheritdoc />
	public DeleteAccountHook(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserAccessor userAccessor,
		IUserManager userManager,
		ISignInManager signInManager)
		: base(contextAccessor, logger, notifier)
	{
		_userAccessor = userAccessor;
		_userManager = userManager;
		_signInManager = signInManager;
	}

	/// <inheritdoc />
	public async Task<HookStatus> Process(DeleteAccountRequest request)
	{
		var userId = _userAccessor.GetUserId();
		if (!userId.HasValue)
		{
			Notifier.Error(ErrorMessages.Account.LoginRequired);
			return HookStatus.Unauthorized;
		}

		var user = await _userManager.GetSienarUser(userId.Value);
		if (user is null)
		{
			Notifier.Error(ErrorMessages.Account.LoginRequired);
			return HookStatus.Unauthorized;
		}

		if (!await _userManager.VerifyPassword(user, request.Password))
		{
			Notifier.Error(ErrorMessages.Account.LoginFailedInvalid);
			return HookStatus.Unauthorized;
		}

		EntitySet.Remove(user);
		await Context.SaveChangesAsync();

		await _signInManager.SignOut();

		return HookStatus.Success;
	}

	/// <inheritdoc />
	public void NotifySuccess()
	{
		Notifier.Success("Account deleted successfully");
	}

	/// <inheritdoc />
	public void NotifyBeforeHookFailure()
	{
		Notifier.Error("Unable to delete account");
	}

	/// <inheritdoc />
	public void NotifyProcessFailure()
	{
		Notifier.Error("An unknown error occurred while deleting your account");
	}

	/// <inheritdoc />
	public void NotifyAfterHookFailure()
	{
		Notifier.Warning("Your account was deleted successfully, but a third party plugin failed to execute");
	}
}