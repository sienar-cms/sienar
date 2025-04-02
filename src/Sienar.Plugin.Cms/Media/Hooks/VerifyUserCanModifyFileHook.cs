using System.Threading.Tasks;
using Sienar.Infrastructure;
using Sienar.Hooks;
using Sienar.Security;

namespace Sienar.Media.Hooks;

public class VerifyUserCanModifyFileHook : IAccessValidator<Upload>
{
	private readonly IUserAccessor _userAccessor;

	public VerifyUserCanModifyFileHook(IUserAccessor userAccessor)
	{
		_userAccessor = userAccessor;
	}

	/// <inheritdoc />
	public async Task Validate(
		AccessValidationContext context,
		ActionType action,
		Upload? input)
	{
		if (action is not (ActionType.Update or ActionType.Delete)) return;

		if (await _userAccessor.UserInRole(Roles.Admin) ||
			await _userAccessor.IsSignedIn() && await _userAccessor.GetUserId() == input?.UserId)
		{
			context.Approve();
		}
	}
}