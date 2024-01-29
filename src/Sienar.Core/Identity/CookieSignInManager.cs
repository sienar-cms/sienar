using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Sienar.Configuration;

namespace Sienar.Identity;

public class CookieSignInManager : ISignInManager
{
	private readonly HttpContext _httpContext;
	private readonly LoginOptions _loginOptions;
	private readonly IUserClaimsPrincipalFactory<SienarUser> _principalFactory;

	public CookieSignInManager(
		IHttpContextAccessor contextAccessor,
		IOptions<LoginOptions> loginOptions,
		IUserClaimsPrincipalFactory<SienarUser> principalFactory)
	{
		_httpContext = contextAccessor.HttpContext 
			?? throw new ArgumentNullException(
				nameof(contextAccessor.HttpContext),
				"HttpContext cannot be null");
		_loginOptions = loginOptions.Value;
		_principalFactory = principalFactory;
	}

	/// <inheritdoc />
	public async Task SignIn(SienarUser user, bool isPersistent)
	{
		var authProperties = new AuthenticationProperties
		{
			IsPersistent = isPersistent,
			AllowRefresh = true,
			IssuedUtc = DateTimeOffset.Now,
			ExpiresUtc = GetExpiration(isPersistent)
		};
		var claimsPrincipal = await _principalFactory.CreateAsync(user);
		await _httpContext.SignInAsync(
			CookieAuthenticationDefaults.AuthenticationScheme,
			claimsPrincipal,
			authProperties);
	}

	public async Task SignOut()
	{
		await _httpContext.SignOutAsync(
			CookieAuthenticationDefaults.AuthenticationScheme);
	}

	private DateTimeOffset GetExpiration(bool isPersistent)
	{
		var duration = isPersistent
			? TimeSpan.FromDays(_loginOptions.PersistentLoginDuration)
			: TimeSpan.FromHours(_loginOptions.TransientLoginDuration);

		return DateTimeOffset.Now + duration;
	}
}