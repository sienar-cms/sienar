using Sienar.Infrastructure;
using Sienar.Security;

namespace Sienar.Hooks;

public class UserIsAdminAccessValidator<T> : UserInRoleAccessValidator<T>
{
	/// <inheritdoc />
	public UserIsAdminAccessValidator(IUserAccessor userAccessor) : base(userAccessor)
	{
		Role = Roles.Admin;
	}
}