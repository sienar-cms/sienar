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
public class DeleteAccountProcessor : DbService<SienarUser>,
	IProcessor<DeleteAccountRequest, bool>
{
	private readonly IUserAccessor _userAccessor;
	private readonly IUserManager _userManager;

	public DeleteAccountProcessor(
		DbContext context,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserAccessor userAccessor,
		IUserManager userManager)
		: base(context, logger, notifier)
	{
		_userAccessor = userAccessor;
		_userManager = userManager;
	}

	public async Task<HookResult<bool>> Process(DeleteAccountRequest request)
	{
		var userId = await _userAccessor.GetUserId();
		if (!userId.HasValue)
		{
			Notifier.Error(ErrorMessages.Account.LoginRequired);
			return this.Unauthorized();
		}

		var user = await _userManager.GetSienarUser(userId.Value);
		if (user is null)
		{
			Notifier.Error(ErrorMessages.Account.LoginRequired);
			return this.Unauthorized();
		}

		if (!await _userManager.VerifyPassword(user, request.Password))
		{
			Notifier.Error(ErrorMessages.Account.LoginFailedInvalid);
			return this.Unauthorized();
		}

		EntitySet.Remove(user);
		await Context.SaveChangesAsync();
		return this.Success(true);
	}

	public void NotifySuccess()
	{
		Notifier.Success("Account deleted successfully");
	}

	public void NotifyFailure()
	{
		Notifier.Error("An unknown error occurred while deleting your account");
	}

	public void NotifyNoPermission()
	{
		Notifier.Error("You do not have permission to delete your account");
	}
}