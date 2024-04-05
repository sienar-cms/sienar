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
public class ConfirmAccountProcessor : DbService<SienarUser>,
	IProcessor<ConfirmAccountRequest, bool>
{
	private readonly IUserManager _userManager;
	private readonly IVerificationCodeManager _vcManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly SienarOptions _options;

	public ConfirmAccountProcessor(
		DbContext context,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager,
		IVerificationCodeManager vcManager,
		IAccountEmailManager emailManager,
		IOptions<SienarOptions> options)
		: base(context, logger, notifier)
	{
		_userManager = userManager;
		_vcManager = vcManager;
		_emailManager = emailManager;
		_options = options.Value;
	}

	public async Task<HookResult<bool>> Process(ConfirmAccountRequest request)
	{
		var user = await _userManager.GetSienarUser(request.UserId);
		if (user is null)
		{
			Notifier.Error(ErrorMessages.Account.AccountErrorInvalidId);
			return this.NotFound();
		}

		var status = await _vcManager.VerifyCode(
			user,
			VerificationCodeTypes.Email,
			request.VerificationCode,
			true);

		if (status == VerificationCodeStatus.Invalid)
		{
			Notifier.Error(ErrorMessages.Account.VerificationCodeInvalid);
			return this.NotFound();
		}

		if (status == VerificationCodeStatus.Expired)
		{
			if (_options.EnableEmail)
			{
				if (await _emailManager.SendWelcomeEmail(user))
				{
					Notifier.Error(ErrorMessages.Account.VerificationCodeExpired);
				}
				else
				{
					Notifier.Error(ErrorMessages.Email.FailedToSend);
				}

				return this.Unprocessable();
			}

			Notifier.Error(
				ErrorMessages.Account.VerificationCodeExpiredEmailDisabled);
			return this.Unprocessable();
		}

		// Code was valid
		user.EmailConfirmed = true;
		EntitySet.Update(user);
		await Context.SaveChangesAsync();

		return this.Success(true);
	}

	public void NotifySuccess()
	{
		Notifier.Success("Account confirmed successfully");
	}

	public void NotifyFailure()
	{
		Notifier.Error("An unknown error occurred while confirming your account");
	}

	public void NotifyNoPermission()
	{
		Notifier.Error("You do not have permission to confirm your account");
	}
}