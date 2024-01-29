using System;

namespace Sienar.Identity.Requests;

public class AddUsertoRoleRequest
{
	public Guid UserId { get; set; }
	public Guid RoleId { get; set; }

	public AddUsertoRoleRequest(Guid userId, Guid roleId)
	{
		UserId = userId;
		RoleId = roleId;
	}
}