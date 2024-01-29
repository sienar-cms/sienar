using System;

namespace Sienar.Identity.Requests;

public class RemoveUserFromRoleRequest
{
	public Guid UserId { get; set; }
	public Guid RoleId { get; set; }

	public RemoveUserFromRoleRequest(Guid userId, Guid roleId)
	{
		UserId = userId;
		RoleId = roleId;
	}
}