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
public class ChangePasswordProcessor : DbService<SienarUser>,
	IProcessor<ChangePasswordRequest, bool>
{
	private readonly IUserManager _userManager;
	private readonly IUserAccessor _userAccessor;

	public ChangePasswordProcessor(
		DbContext context,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager,
		IUserAccessor userAccessor)
		: base(context, logger, notifier)
	{
		_userManager = userManager;
		_userAccessor = userAccessor;
	}

	public async Task<HookResult<bool>> Process(ChangePasswordRequest request)
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

		if (!await _userManager.VerifyPassword(user, request.CurrentPassword))
		{
			Notifier.Error(ErrorMessages.Account.LoginFailedInvalid);
			return this.Unauthorized();
		}

		await _userManager.UpdatePassword(user, request.NewPassword);

		return this.Success(true);
	}

	public void NotifySuccess()
	{
		Notifier.Success("Password changed successfully");
	}

	public void NotifyFailure()
	{
		Notifier.Error("An unknown error occurred while changing your password");
	}

	public void NotifyNoPermission()
	{
		Notifier.Error("You do not have permission to change your password");
	}
}