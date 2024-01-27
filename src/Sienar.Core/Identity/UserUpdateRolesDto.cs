#nullable disable

using System;
using System.ComponentModel.DataAnnotations;

namespace Sienar.Identity;

public class UserUpdateRolesDto
{
	[Required]
	public Guid UserId { get; set; }

	[Required]
	public Guid RoleId { get; set; }
}