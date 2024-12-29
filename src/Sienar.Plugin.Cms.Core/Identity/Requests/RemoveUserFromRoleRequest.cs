using System;

namespace Sienar.Identity.Requests;

public class RemoveUserFromRoleRequest
{
	public Guid UserId { get; set; }
	public Guid RoleId { get; set; }
}