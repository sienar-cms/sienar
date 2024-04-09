using System;

namespace Sienar.Identity.Requests;

public class AddUserToRoleRequest
{
	public Guid UserId { get; set; }
	public Guid RoleId { get; set; }

	public AddUserToRoleRequest(Guid userId, Guid roleId)
	{
		UserId = userId;
		RoleId = roleId;
	}
}