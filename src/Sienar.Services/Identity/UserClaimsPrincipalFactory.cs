using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Sienar.Identity;

public class UserClaimsPrincipalFactory
	: IUserClaimsPrincipalFactory<SienarUser>
{
	protected readonly IUserClaimsFactory ClaimsFactory;

	public UserClaimsPrincipalFactory(IUserClaimsFactory claimsFactory)
	{
		ClaimsFactory = claimsFactory;
	}

	public virtual async Task<ClaimsPrincipal> CreateAsync(SienarUser user)
	{
		var identity = await GenerateClaims(user);
		return new ClaimsPrincipal(identity);
	}

	protected virtual Task<ClaimsIdentity> GenerateClaims(SienarUser user)
	{
		var identity = new ClaimsIdentity(
			ClaimsFactory.CreateClaims(user),
			CookieAuthenticationDefaults.AuthenticationScheme);
		return Task.FromResult(identity);
	}
}