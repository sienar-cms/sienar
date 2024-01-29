using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Sienar.Identity;

public class UserClaimsPrincipalFactory
	: IUserClaimsPrincipalFactory<SienarUser>
{
	private readonly IUserClaimsFactory _claimsFactory;

	public UserClaimsPrincipalFactory(IUserClaimsFactory claimsFactory)
	{
		_claimsFactory = claimsFactory;
	}

	public async Task<ClaimsPrincipal> CreateAsync(SienarUser user)
	{
		var identity = await GenerateClaims(user);
		return new ClaimsPrincipal(identity);
	}

	private Task<ClaimsIdentity> GenerateClaims(SienarUser user)
	{
		var identity = new ClaimsIdentity(
			_claimsFactory.CreateClaims(user),
			CookieAuthenticationDefaults.AuthenticationScheme);
		return Task.FromResult(identity);
	}
}