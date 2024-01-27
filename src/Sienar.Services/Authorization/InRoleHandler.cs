using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Sienar.Authorization;

public class InRoleHandler : AuthorizationHandler<InRoleRequirement>
{
	protected override Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		InRoleRequirement requirement)
	{
		if (context.User.IsInRole(requirement.RoleName))
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}