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

public class PerformEmailChangeProcessor : DbService<SienarUser>,
	IProcessor<PerformEmailChangeRequest>
{
	private readonly IUserManager _userManager;
	private readonly IUserAccessor _userAccessor;
	private readonly IVerificationCodeManager _vcManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly SienarOptions _sienarOptions;

	/// <inheritdoc />
	public PerformEmailChangeProcessor(IDbContextAccessor<DbContext> contextAccessor, ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager,
		IUserAccessor userAccessor,
		IVerificationCodeManager vcManager,
		IAccountEmailManager emailManager,
		IOptions<SienarOptions> sienarOptions) : base(contextAccessor, logger, notifier)
	{
		_userManager = userManager;
		_userAccessor = userAccessor;
		_vcManager = vcManager;
		_emailManager = emailManager;
		_sienarOptions = sienarOptions.Value;
	}

	/// <inheritdoc />
	public async Task<HookStatus> Process(PerformEmailChangeRequest request)
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

		if (user.Id != request.UserId)
		{
			Notifier.Error(ErrorMessages.Account.AccountErrorWrongId);
			return HookStatus.Unprocessable;
		}

		if (string.IsNullOrEmpty(user.PendingEmail))
		{
			Notifier.Error($"Unable to change email address because you have no pending email change");
			return HookStatus.Unprocessable;
		}

		var status = await _vcManager.VerifyCode(
			user,
			VerificationCodeTypes.ChangeEmail,
			request.VerificationCode,
			true);

		if (status == VerificationCodeStatus.Invalid)
		{
			Notifier.Error(ErrorMessages.Account.VerificationCodeInvalid);
			return HookStatus.NotFound;
		}

		if (status == VerificationCodeStatus.Expired)
		{
			if (_sienarOptions.EnableEmail)
			{
				await user.SendEmailChangeConfirmationEmail(
					_vcManager,
					_emailManager);
				Notifier.Error(ErrorMessages.Account.VerificationCodeExpired);
				return HookStatus.Unprocessable;
			}

			Notifier.Error(
				ErrorMessages.Account.VerificationCodeExpiredEmailDisabled);
			return HookStatus.Unprocessable;
		}

		// Code was valid
		user.Email = user.PendingEmail;
		user.PendingEmail = null;

		EntitySet.Update(user);
		await Context.SaveChangesAsync();

		return HookStatus.Success;
	}

	/// <inheritdoc />
	public void NotifySuccess()
	{
		Notifier.Success("Email changed successfully");
	}

	/// <inheritdoc />
	public void NotifyProcessFailure()
	{
		Notifier.Error("An unknown error occurred while changing your email");
	}
}