using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace Sienar.Infrastructure;

public class DefaultAuthStateProvider : AuthenticationStateProvider
{
	public override Task<AuthenticationState> GetAuthenticationStateAsync()
		=> Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
}