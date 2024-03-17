using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace Sienar.Infrastructure;

public class AuthStateProvider : AuthenticationStateProvider
{
	private Task<AuthenticationState> _authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));

	/// <inheritdoc />
	public override Task<AuthenticationState> GetAuthenticationStateAsync()
		=> _authState;
}