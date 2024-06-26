#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Email;
using Sienar.Errors;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class LoginProcessor : IProcessor<LoginRequest, Guid>
{
	private readonly DbContext _context;
	private readonly IUserManager _userManager;
	private readonly LoginTokenCache _tokenCache;
	private readonly IAccountEmailManager _emailManager;
	private readonly LoginOptions _loginOptions;
	private readonly SienarOptions _appOptions;

	public LoginProcessor(
		DbContext context,
		IUserManager userManager,
		LoginTokenCache tokenCache,
		IAccountEmailManager emailManager,
		IOptions<LoginOptions> loginOptions,
		IOptions<SienarOptions> appOptions)
	{
		_context = context;
		_userManager = userManager;
		_tokenCache = tokenCache;
		_emailManager = emailManager;
		_loginOptions = loginOptions.Value;
		_appOptions = appOptions.Value;
	}

	public async Task<OperationResult<Guid>> Process(LoginRequest request)
	{
		var entitySet = _context.Set<SienarUser>();

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

			entitySet.Update(user);
			await _context.SaveChangesAsync();

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
		entitySet.Update(user);
		await _context.SaveChangesAsync();

		// Save the token to the token cache
		var loginToken = Guid.NewGuid();
		_tokenCache.AddLoginToken(loginToken, request);

		return this.Success(loginToken, "Logged in successfully");
	}
}