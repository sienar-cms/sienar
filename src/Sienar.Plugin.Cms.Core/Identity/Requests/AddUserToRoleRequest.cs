using System;
using Sienar.Services;

namespace Sienar.Identity.Requests;

public class AddUserToRoleRequest : IRequest
{
	public Guid UserId { get; set; }
	public Guid RoleId { get; set; }
}