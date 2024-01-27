using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

// ReSharper disable once CheckNamespace
namespace Sienar.Identity;

public class AuthStateProvider : AuthenticationStateProvider
{
	private AuthenticationState? _authState;
	private readonly IUserClaimsFactory _userClaimsFactory;

	/// <inheritdoc />
	public AuthStateProvider(IUserClaimsFactory userClaimsFactory)
	{
		_userClaimsFactory = userClaimsFactory;
	}

	public override Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		_authState ??= CreateAuthStateFromClaims(Array.Empty<Claim>(), false);
		return Task.FromResult(_authState);
	}

	/// <summary>
	/// Notifies auth state listeners of an authentication state change
	/// </summary>
	/// <param name="user">The user object describing the new authentication state. Null if the user is de-authenticated</param>
	public void NotifyUserAuthentication(SienarUser? user)
	{
		var isAuthenticated = user is not null;
		var claims = isAuthenticated
			? _userClaimsFactory.CreateClaims(user!)
			: Array.Empty<Claim>();

		_authState = CreateAuthStateFromClaims(claims, isAuthenticated);
		NotifyAuthenticationStateChanged(Task.FromResult(_authState));
	}

	private static AuthenticationState CreateAuthStateFromClaims(
		IEnumerable<Claim> claims,
		bool isAuthenticated)
	{
		var identity = isAuthenticated
			? new ClaimsIdentity(claims, "BlazorServerBrowserAuth")
			: new ClaimsIdentity(claims);

		var principal = new ClaimsPrincipal(identity);
		return new AuthenticationState(principal);
	}
}