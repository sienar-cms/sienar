using System;
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

public class LoginProcessor : DbService<SienarUser>, 
	IProcessor<LoginRequest>
{
	private readonly IUserManager _userManager;
	private readonly ISignInManager _signInManager;
	private readonly IVerificationCodeManager _verificationCodeManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly LoginOptions _loginOptions;
	private readonly SienarOptions _appOptions;

	/// <inheritdoc />
	public LoginProcessor(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager,
		ISignInManager signInManager,
		IVerificationCodeManager verificationCodeManager,
		IAccountEmailManager emailManager,
		IOptions<LoginOptions> loginOptions,
		IOptions<SienarOptions> appOptions)
		: base(contextAccessor, logger, notifier)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_verificationCodeManager = verificationCodeManager;
		_emailManager = emailManager;
		_loginOptions = loginOptions.Value;
		_appOptions = appOptions.Value;
	}

	/// <inheritdoc />
	public async Task<HookStatus> Process(LoginRequest request)
	{
		var user = await _userManager.GetSienarUser(
			request.AccountName,
			u => u.Roles);
		if (user == null)
		{
			Notifier.Error(ErrorMessages.Account.LoginFailedNotFound);
			return HookStatus.NotFound;
		}

		// If user is locked out, tell them when the lockout date ends
		if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.Now)
		{
			Notifier.Error(ErrorMessages.Account.GetLockoutMessage(user.LockoutEnd));
			return HookStatus.Unauthorized;
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
			return HookStatus.Unauthorized;
		}

		// Still check if email is confirmed no matter what
		// That way, users registered when confirmation was
		// required still need to confirm
		if (!user.EmailConfirmed)
		{
			if (_appOptions.EnableEmail)
			{
				await user.SendWelcomeEmail(
					_verificationCodeManager,
					_emailManager);
				Notifier.Warning(ErrorMessages.Account.LoginFailedNotConfirmed);
				return HookStatus.Unauthorized;
			}

			Notifier.Error(
				ErrorMessages.Account.LoginFailedNotConfirmedEmailDisabled);
			return HookStatus.Unauthorized;
		}

		// User is authenticated and able to log in
		user.LoginFailedCount = 0;
		EntitySet.Update(user);
		await Context.SaveChangesAsync();

		await _signInManager.SignIn(user, request.RememberMe);

		return HookStatus.Success;
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