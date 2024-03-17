using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Sienar.Identity;

public class UserClaimsFactory : IUserClaimsFactory
{
	/// <inheritdoc />
	public IEnumerable<Claim> CreateClaims(SienarUser user)
	{
		var claims = new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new(ClaimTypes.Name, user.Username),
			new(ClaimTypes.Email, user.Email)
		};

		if (user.Roles.Count > 0)
		{
			claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r.Name)));
		}

		return claims;
	}
}