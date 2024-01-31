using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Email;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;
using Sienar.Infrastructure.Services;

namespace Sienar.Identity.Processors;

public class ConfirmAccountProcessor : DbService<SienarUser>,
	IProcessor<ConfirmAccountRequest>
{
	private readonly IUserManager _userManager;
	private readonly IVerificationCodeManager _vcManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly SienarOptions _options;

	/// <inheritdoc />
	public ConfirmAccountProcessor(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager,
		IVerificationCodeManager vcManager,
		IAccountEmailManager emailManager,
		IOptions<SienarOptions> options)
		: base(contextAccessor, logger, notifier)
	{
		_userManager = userManager;
		_vcManager = vcManager;
		_emailManager = emailManager;
		_options = options.Value;
	}

	/// <inheritdoc />
	public async Task<HookStatus> Process(ConfirmAccountRequest request)
	{
		var user = await _userManager.GetSienarUser(request.UserId);
		if (user is null)
		{
			Notifier.Error(ErrorMessages.Account.AccountErrorInvalidId);
			return HookStatus.NotFound;
		}

		var status = await _vcManager.VerifyCode(
			user,
			VerificationCodeTypes.Email,
			request.VerificationCode,
			true);

		if (status == VerificationCodeStatus.Invalid)
		{
			Notifier.Error(ErrorMessages.Account.VerificationCodeInvalid);
			return HookStatus.NotFound;
		}

		if (status == VerificationCodeStatus.Expired)
		{
			if (_options.EnableEmail)
			{
				await user.SendWelcomeEmail(_vcManager, _emailManager);
				Notifier.Error(ErrorMessages.Account.VerificationCodeExpired);
				return HookStatus.Unprocessable;
			}

			Notifier.Error(
				ErrorMessages.Account.VerificationCodeExpiredEmailDisabled);
			return HookStatus.Unprocessable;
		}

		// Code was valid
		user.EmailConfirmed = true;
		EntitySet.Update(user);
		await Context.SaveChangesAsync();

		return HookStatus.Success;
	}

	/// <inheritdoc />
	public void NotifySuccess()
	{
		Notifier.Success("Account confirmed successfully");
	}

	/// <inheritdoc />
	public void NotifyFailure()
	{
		Notifier.Error("An unknown error occurred while confirming your account");
	}

	/// <inheritdoc />
	public void NotifyNoPermission()
	{
		Notifier.Error("You do not have permission to confirm your account");
	}
}