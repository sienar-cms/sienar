using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

// ReSharper disable once CheckNamespace
namespace Sienar.Identity;

public class AuthStateProvider : AuthenticationStateProvider
{
	private AuthenticationState? _authState;
	private readonly IUserClaimsFactory _userClaimsFactory;
	private readonly IBlazorLoginDataManager _loginDataManager;
	private readonly AccountStateProvider _accountState;

	/// <inheritdoc />
	public AuthStateProvider(
		IUserClaimsFactory userClaimsFactory,
		IBlazorLoginDataManager loginDataManager,
		AccountStateProvider accountState)
	{
		_userClaimsFactory = userClaimsFactory;
		_loginDataManager = loginDataManager;
		_accountState = accountState;
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		if (_authState is null)
		{
			try
			{
				var user = await _loginDataManager.LoadUserLoginStatus();
				_authState = CreateAuthState(user);
				_accountState.User = user;
			}
			// JS interop not available
			catch (InvalidOperationException)
			{
				_authState = CreateAuthState(null);
			}
		}

		return _authState;
	}

	/// <summary>
	/// Notifies auth state listeners of an authentication state change
	/// </summary>
	/// <param name="user">The user object describing the new authentication state. Null if the user is de-authenticated</param>
	public void NotifyUserAuthentication(SienarUser? user)
	{
		_authState = CreateAuthState(user);
		NotifyAuthenticationStateChanged(Task.FromResult(_authState));
	}

	private AuthenticationState CreateAuthState(SienarUser? user)
	{
		var isAuthenticated = user is not null;
		var claims = isAuthenticated
			? _userClaimsFactory.CreateClaims(user!)
			: Array.Empty<Claim>();

		var identity = isAuthenticated
			? new ClaimsIdentity(claims, "BlazorServerBrowserAuth")
			: new ClaimsIdentity(claims);

		var principal = new ClaimsPrincipal(identity);
		return new AuthenticationState(principal);
	}
}