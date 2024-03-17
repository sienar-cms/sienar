using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace Sienar.Infrastructure;

public class MauiUserAccessor : IUserAccessor
{
	private AuthenticationStateProvider _authStateProvider;

	public MauiUserAccessor(AuthenticationStateProvider authStateProvider)
	{
		_authStateProvider = authStateProvider;
	}

	/// <inheritdoc />
	public async Task<bool> IsSignedIn()
	{
		var state = await _authStateProvider.GetAuthenticationStateAsync();
		return state.User.Identity?.IsAuthenticated ?? false;
	}

	/// <inheritdoc />
	public async Task<Guid?> GetUserId()
	{
		var state = await _authStateProvider.GetAuthenticationStateAsync();
		var claim = state.User.Claims.FirstOrDefault(
			c => c.Type == ClaimTypes.NameIdentifier);
		return claim is null
			? null
			: Guid.Parse(claim.Value);
	}

	/// <inheritdoc />
	public async Task<string?> GetUsername()
	{
		var state = await _authStateProvider.GetAuthenticationStateAsync();
		return state.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
	}

	/// <inheritdoc />
	public async Task<ClaimsPrincipal> GetUserClaimsPrincipal()
	{
		var state = await _authStateProvider.GetAuthenticationStateAsync();
		return state.User;
	}

	/// <inheritdoc />
	public async Task<bool> UserInRole(string roleName)
	{
		var state = await _authStateProvider.GetAuthenticationStateAsync();
		return state.User.IsInRole(roleName);
	}
}