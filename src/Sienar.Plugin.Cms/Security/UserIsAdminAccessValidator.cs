using Sienar.Infrastructure;

namespace Sienar.Security;

public class UserIsAdminAccessValidator<T> : UserInRoleAccessValidator<T>
{
	/// <inheritdoc />
	public UserIsAdminAccessValidator(IUserAccessor userAccessor) : base(userAccessor)
	{
		Role = Roles.Admin;
	}
}