using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace Sienar.Infrastructure;

/// <summary>
/// A WASM-friendly implementation of <see cref="AuthenticationStateProvider"/>
/// </summary>
public class AuthStateProvider : AuthenticationStateProvider
{
	private AuthenticationState _authState = CreateAuthenticationState([], false);

	/// <ignore />
	public override Task<AuthenticationState> GetAuthenticationStateAsync()
		=> Task.FromResult(_authState);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="claims"></param>
	/// <param name="isAuthenticated"></param>
	public void NotifyUserAuthentication(
		IEnumerable<Claim> claims,
		bool isAuthenticated)
	{
		_authState = CreateAuthenticationState(claims, isAuthenticated);
		NotifyAuthenticationStateChanged(Task.FromResult(_authState));
	}

	private static AuthenticationState CreateAuthenticationState(
		IEnumerable<Claim> claims,
		bool isAuthenticated)
	{
		var identity = isAuthenticated
			? new ClaimsIdentity(claims, "CookieAuth")
			: new ClaimsIdentity(claims);
		var principal = new ClaimsPrincipal(identity);
		return new AuthenticationState(principal);
	}
}
