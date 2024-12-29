using System.Threading.Tasks;
using Sienar.Infrastructure;
using Sienar.Hooks;

namespace Sienar.Media.Hooks;

public class VerifyUserCanReadFileHook : IAccessValidator<Upload>
{
	private readonly IUserAccessor _userAccessor;

	public VerifyUserCanReadFileHook(IUserAccessor userAccessor)
	{
		_userAccessor = userAccessor;
	}

	/// <inheritdoc />
	public async Task Validate(
		AccessValidationContext context,
		ActionType action,
		Upload? input)
	{
		if (action is not (ActionType.Read or ActionType.ReadAll) || input is null) return;

		if (input.UserId is null
			|| await _userAccessor.UserInRole(Roles.Admin)
			|| await _userAccessor.IsSignedIn() && await _userAccessor.GetUserId() == input.UserId
			|| !input.IsPrivate)
		{
			context.Approve();
		}
	}
}