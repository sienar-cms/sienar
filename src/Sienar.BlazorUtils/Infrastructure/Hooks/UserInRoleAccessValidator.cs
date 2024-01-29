using System.Threading.Tasks;

namespace Sienar.Infrastructure.Hooks;

public abstract class UserInRoleAccessValidator<T> : IAccessValidator<T>
{
	private readonly IUserAccessor _userAccessor;
	protected string Role = string.Empty;

	protected UserInRoleAccessValidator(IUserAccessor userAccessor)
	{
		_userAccessor = userAccessor;
	}

	/// <inheritdoc />
	public Task Validate(UserAccessValidationContext context, T? input)
	{
		if (_userAccessor.UserInRole(Role))
		{
			context.Approve();
		}

		return Task.CompletedTask;
	}
}