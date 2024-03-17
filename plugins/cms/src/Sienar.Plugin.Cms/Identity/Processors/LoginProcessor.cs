using System;
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

public class LoginProcessor : DbService<SienarUser>, 
	IProcessor<LoginRequest, Guid>
{
	private readonly IUserManager _userManager;
	private readonly LoginTokenCache _tokenCache;
	private readonly IVerificationCodeManager _verificationCodeManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly LoginOptions _loginOptions;
	private readonly SienarOptions _appOptions;

	/// <inheritdoc />
	public LoginProcessor(
		DbContext context,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager,
		LoginTokenCache tokenCache,
		IVerificationCodeManager verificationCodeManager,
		IAccountEmailManager emailManager,
		IOptions<LoginOptions> loginOptions,
		IOptions<SienarOptions> appOptions)
		: base(context, logger, notifier)
	{
		_userManager = userManager;
		_tokenCache = tokenCache;
		_verificationCodeManager = verificationCodeManager;
		_emailManager = emailManager;
		_loginOptions = loginOptions.Value;
		_appOptions = appOptions.Value;
	}

	/// <inheritdoc />
	public async Task<HookResult<Guid>> Process(LoginRequest request)
	{
		var user = await _userManager.GetSienarUser(
			request.AccountName,
			u => u.Roles);
		if (user == null)
		{
			Notifier.Error(ErrorMessages.Account.LoginFailedNotFound);
			return this.NotFound();
		}

		// If user is locked out, tell them when the lockout date ends
		if (user.IsLockedOut())
		{
			Notifier.Error(ErrorMessages.Account.LoginFailedLocked);
			return this.Unauthorized();
		}

		if (!await _userManager.VerifyPassword(user, request.Password))
		{
			user.LoginFailedCount++;
			if (user.LoginFailedCount >= _loginOptions.MaxFailedLoginAttempts)
			{
				user.LoginFailedCount = 0;
				user.LockoutEnd = DateTime.Now + _loginOptions.LockoutTimespan;
			}

			EntitySet.Update(user);
			await Context.SaveChangesAsync();

			Notifier.Error(ErrorMessages.Account.LoginFailedInvalid);
			return this.Unauthorized();
		}

		// Still check if email is confirmed no matter what
		// That way, users registered when confirmation was
		// required still need to confirm
		if (!user.EmailConfirmed)
		{
			if (_appOptions.EnableEmail)
			{
				await _emailManager.SendWelcomeEmail(
					_verificationCodeManager,
					user);
				Notifier.Warning(ErrorMessages.Account.LoginFailedNotConfirmed);
				return this.Unauthorized();
			}

			Notifier.Error(
				ErrorMessages.Account.LoginFailedNotConfirmedEmailDisabled);
			return this.Unauthorized();
		}

		// User is authenticated and able to log in
		user.LoginFailedCount = 0;
		EntitySet.Update(user);
		await Context.SaveChangesAsync();

		// Save the token to the token cache
		var loginToken = Guid.NewGuid();
		_tokenCache.AddLoginToken(loginToken, request);

		return this.Success(loginToken);
	}

	/// <inheritdoc />
	public void NotifySuccess()
	{
		Notifier.Success("Logged in successfully");
	}

	/// <inheritdoc />
	public void NotifyFailure()
	{
		Notifier.Error("An unknown error occurred while logging in");
	}

	/// <inheritdoc />
	public void NotifyNoPermission()
	{
		Notifier.Error("You do not have permission to log in");
	}
}