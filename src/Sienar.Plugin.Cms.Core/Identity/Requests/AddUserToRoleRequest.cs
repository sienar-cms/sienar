using System;

namespace Sienar.Identity.Requests;

public class AddUserToRoleRequest
{
	public Guid UserId { get; set; }
	public Guid RoleId { get; set; }
}