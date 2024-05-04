#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;
using Sienar.Infrastructure.Services;

namespace Sienar.Identity.Processors;

/// <exclude />
public class LoginProcessor : DbService<SienarUser>, 
	IProcessor<LoginRequest, Guid>
{
	private readonly IUserManager _userManager;
	private readonly LoginTokenCache _tokenCache;
	private readonly IAccountEmailManager _emailManager;
	private readonly LoginOptions _loginOptions;
	private readonly SienarOptions _appOptions;

	public LoginProcessor(
		DbContext context,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager,
		LoginTokenCache tokenCache,
		IAccountEmailManager emailManager,
		IOptions<LoginOptions> loginOptions,
		IOptions<SienarOptions> appOptions)
		: base(context, logger, notifier)
	{
		_userManager = userManager;
		_tokenCache = tokenCache;
		_emailManager = emailManager;
		_loginOptions = loginOptions.Value;
		_appOptions = appOptions.Value;
	}

	public async Task<OperationResult<Guid>> Process(LoginRequest request)
	{
		var user = await _userManager.GetSienarUser(
			request.AccountName,
			u => u.Roles);
		if (user == null)
		{
			return this.NotFound(message: CmsErrors.Account.LoginFailedNotFound);
		}

		// If user is locked out, tell them when the lockout date ends
		if (user.IsLockedOut())
		{
			return this.Unauthorized(message: CmsErrors.Account.LoginFailedLocked);
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

			return this.Unauthorized(message: CmsErrors.Account.LoginFailedInvalid);
		}

		// Still check if email is confirmed no matter what
		// That way, users registered when confirmation was
		// required still need to confirm
		if (!user.EmailConfirmed)
		{
			if (_appOptions.EnableEmail)
			{
				await _emailManager.SendWelcomeEmail(user);
				return this.Unauthorized(message: CmsErrors.Account.LoginFailedNotConfirmed);
			}

			return this.Unauthorized(message: CmsErrors.Account.LoginFailedNotConfirmedEmailDisabled);
		}

		// User is authenticated and able to log in
		user.LoginFailedCount = 0;
		EntitySet.Update(user);
		await Context.SaveChangesAsync();

		// Save the token to the token cache
		var loginToken = Guid.NewGuid();
		_tokenCache.AddLoginToken(loginToken, request);

		return this.Success(loginToken, "Logged in successfully");
	}
}