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

public class InitiateEmailChangeProcessor : DbService<SienarUser>,
	IProcessor<InitiateEmailChangeRequest, bool>
{
	private readonly IUserManager _userManager;
	private readonly IVerificationCodeManager _vcManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly IUserAccessor _userAccessor;
	private readonly SienarOptions _sienarOptions;
	private readonly LoginOptions _loginOptions;

	/// <inheritdoc />
	public InitiateEmailChangeProcessor(
		DbContext context,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager,
		IVerificationCodeManager vcManager,
		IAccountEmailManager emailManager,
		IUserAccessor userAccessor,
		IOptions<SienarOptions> sienarOptions,
		IOptions<LoginOptions> loginOptions) : base(context, logger, notifier)
	{
		_userManager = userManager;
		_vcManager = vcManager;
		_emailManager = emailManager;
		_userAccessor = userAccessor;
		_sienarOptions = sienarOptions.Value;
		_loginOptions = loginOptions.Value;
	}

	/// <inheritdoc />
	public async Task<HookResult<bool>> Process(InitiateEmailChangeRequest request)
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

		if (!await _userManager.VerifyPassword(user, request.ConfirmPassword))
		{
			Notifier.Error(ErrorMessages.Account.LoginFailedInvalid);
			return this.Unauthorized();
		}

		var shouldSendConfirmationEmail = SienarUserExtensions.ShouldSendEmailConfirmationEmail(
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
			await _emailManager.SendEmailChangeConfirmationEmail(_vcManager, user);
		}

		return this.Success(true);
	}

	/// <inheritdoc />
	public void NotifySuccess()
	{
		Notifier.Success("Email change requested");
	}

	/// <inheritdoc />
	public void NotifyFailure()
	{
		Notifier.Error("An unknown error occurred while requesting an email change");
	}

	/// <inheritdoc />
	public void NotifyNoPermission()
	{
		Notifier.Error("You do not have permission to change your email");
	}
}