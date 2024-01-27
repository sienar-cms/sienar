using Microsoft.AspNetCore.Authorization;

namespace Sienar.Authorization;

public class InRoleRequirement : IAuthorizationRequirement
{
	public string RoleName { get; }

	public InRoleRequirement(string roleName)
	{
		RoleName = roleName;
	}
}