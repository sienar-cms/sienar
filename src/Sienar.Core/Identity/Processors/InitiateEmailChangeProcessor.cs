using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;
using Sienar.Infrastructure.Services;

namespace Sienar.Identity.Processors;

public class InitiateEmailChangeProcessor : DbService<SienarUser>,
	IProcessor<InitiateEmailChangeRequest>
{
	private readonly IUserManager _userManager;
	private readonly IVerificationCodeManager _vcManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly IUserAccessor _userAccessor;
	private readonly SienarOptions _sienarOptions;
	private readonly LoginOptions _loginOptions;

	/// <inheritdoc />
	public InitiateEmailChangeProcessor(IDbContextAccessor<DbContext> contextAccessor, ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager,
		IVerificationCodeManager vcManager,
		IAccountEmailManager emailManager,
		IUserAccessor userAccessor,
		IOptions<SienarOptions> sienarOptions,
		IOptions<LoginOptions> loginOptions) : base(contextAccessor, logger, notifier)
	{
		_userManager = userManager;
		_vcManager = vcManager;
		_emailManager = emailManager;
		_userAccessor = userAccessor;
		_sienarOptions = sienarOptions.Value;
		_loginOptions = loginOptions.Value;
	}

	/// <inheritdoc />
	public async Task<HookStatus> Process(InitiateEmailChangeRequest request)
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

		if (!await _userManager.VerifyPassword(user, request.ConfirmPassword))
		{
			Notifier.Error(ErrorMessages.Account.LoginFailedInvalid);
			return HookStatus.Unauthorized;
		}

		var shouldSendConfirmationEmail = AccountUtils.ShouldSendEmailConfirmationEmail(
			_loginOptions,
			_sienarOptions);

		if (shouldSendConfirmationEmail)
		{
			user.PendingEmail = request.Email;
		}
		else
		{
			user.Email = request.Email;
		}

		EntitySet.Update(user);
		await Context.SaveChangesAsync();

		if (shouldSendConfirmationEmail)
		{
			await user.SendEmailChangeConfirmationEmail(_vcManager, _emailManager);
		}

		return HookStatus.Success;
	}

	/// <inheritdoc />
	public void NotifySuccess()
	{
		Notifier.Success("Email change requested");
	}

	/// <inheritdoc />
	public void NotifyBeforeHookFailure()
	{
		Notifier.Error("Unable to request an email change");
	}

	/// <inheritdoc />
	public void NotifyProcessFailure()
	{
		Notifier.Error("An unknown error occurred while requesting an email change");
	}

	/// <inheritdoc />
	public void NotifyAfterHookFailure()
	{
		Notifier.Warning("Your email change was requested successfully, but a third party plugin failed to execute");
	}
}