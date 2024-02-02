using System.Threading.Tasks;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Media.Hooks;

public class VerifyUserCanModifyFileHook : IAccessValidator<Upload>
{
	private readonly IUserAccessor _userAccessor;

	public VerifyUserCanModifyFileHook(IUserAccessor userAccessor)
	{
		_userAccessor = userAccessor;
	}

	/// <inheritdoc />
	public Task Validate(
		AccessValidationContext context,
		ActionType action,
		Upload? input)
	{
		if (action is not (ActionType.Update or ActionType.Delete))
			return Task.CompletedTask;

		if (_userAccessor.UserInRole(Roles.Admin)
			|| _userAccessor.IsSignedIn() && _userAccessor.GetUserId() == input?.UserId)
			context.Approve();

		return Task.CompletedTask;
	}
}