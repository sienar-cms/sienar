using Sienar.Services;

namespace Sienar.Identity.Requests;

public class AddUserToRoleRequest : IRequest
{
	public int UserId { get; set; }
	public int RoleId { get; set; }
}