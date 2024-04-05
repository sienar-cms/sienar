#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Email;
using Sienar.Errors;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;
using Sienar.Infrastructure.Services;

namespace Sienar.Identity.Processors;

/// <exclude />
public class PerformEmailChangeProcessor : DbService<SienarUser>,
	IProcessor<PerformEmailChangeRequest, bool>
{
	private readonly IUserManager _userManager;
	private readonly IUserAccessor _userAccessor;
	private readonly IVerificationCodeManager _vcManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly SienarOptions _sienarOptions;

	public PerformEmailChangeProcessor(
		DbContext context,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager,
		IUserAccessor userAccessor,
		IVerificationCodeManager vcManager,
		IAccountEmailManager emailManager,
		IOptions<SienarOptions> sienarOptions) : base(context, logger, notifier)
	{
		_userManager = userManager;
		_userAccessor = userAccessor;
		_vcManager = vcManager;
		_emailManager = emailManager;
		_sienarOptions = sienarOptions.Value;
	}

	public async Task<HookResult<bool>> Process(PerformEmailChangeRequest request)
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

		if (user.Id != request.UserId)
		{
			Notifier.Error(ErrorMessages.Account.AccountErrorWrongId);
			return this.Unprocessable();
		}

		if (string.IsNullOrEmpty(user.PendingEmail))
		{
			Notifier.Error($"Unable to change email address because you have no pending email change");
			return this.Unprocessable();
		}

		var status = await _vcManager.VerifyCode(
			user,
			VerificationCodeTypes.ChangeEmail,
			request.VerificationCode,
			true);

		if (status == VerificationCodeStatus.Invalid)
		{
			Notifier.Error(ErrorMessages.Account.VerificationCodeInvalid);
			return this.NotFound();
		}

		if (status == VerificationCodeStatus.Expired)
		{
			if (_sienarOptions.EnableEmail)
			{
				await _emailManager.SendEmailChangeConfirmationEmail(user);
				Notifier.Error(ErrorMessages.Account.VerificationCodeExpired);
				return this.Unprocessable();
			}

			Notifier.Error(
				ErrorMessages.Account.VerificationCodeExpiredEmailDisabled);
			return this.Unprocessable();
		}

		// Code was valid
		user.Email = user.PendingEmail;
		user.PendingEmail = null;

		EntitySet.Update(user);
		await Context.SaveChangesAsync();

		return this.Success(true);
	}

	public void NotifySuccess()
	{
		Notifier.Success("Email changed successfully");
	}

	public void NotifyFailure()
	{
		Notifier.Error("An unknown error occurred while changing your email");
	}

	public void NotifyNoPermission()
	{
		Notifier.Error("You do not have permission to change your email");
	}
}