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

public class ChangePasswordProcessor : DbService<SienarUser>,
	IProcessor<ChangePasswordRequest>
{
	private readonly IUserManager _userManager;
	private readonly IUserAccessor _userAccessor;

	/// <inheritdoc />
	public ChangePasswordProcessor(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager,
		IUserAccessor userAccessor)
		: base(contextAccessor, logger, notifier)
	{
		_userManager = userManager;
		_userAccessor = userAccessor;
	}

	/// <inheritdoc />
	public async Task<HookStatus> Process(ChangePasswordRequest request)
	{
		var userId = _userAccessor.GetUserId();
		if (!userId.HasValue)
		{
			Notifier.Error(ErrorMessages.Account.LoginRequired);
			return HookStatus.Unauthorized;
		}

		var user = await _userManager.GetSienarUser(userId!.Value);
		if (user is null)
		{
			Notifier.Error(ErrorMessages.Account.LoginRequired);
			return HookStatus.Unauthorized;
		}

		if (!await _userManager.VerifyPassword(user, request.CurrentPassword))
		{
			Notifier.Error(ErrorMessages.Account.LoginFailedInvalid);
			return HookStatus.Unauthorized;
		}

		await _userManager.UpdatePassword(user, request.NewPassword);

		return HookStatus.Success;
	}

	/// <inheritdoc />
	public void NotifySuccess()
	{
		Notifier.Success("Password changed successfully");
	}

	/// <inheritdoc />
	public void NotifyProcessFailure()
	{
		Notifier.Error("An unknown error occurred while changing your password");
	}
}