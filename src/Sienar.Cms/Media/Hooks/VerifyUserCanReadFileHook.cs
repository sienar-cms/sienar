using System.Threading.Tasks;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Media.Hooks;

public class VerifyUserCanReadFileHook : IAccessValidator<Upload>
{
	private readonly IUserAccessor _userAccessor;

	public VerifyUserCanReadFileHook(IUserAccessor userAccessor)
	{
		_userAccessor = userAccessor;
	}

	/// <inheritdoc />
	public Task Validate(
		AccessValidationContext context,
		ActionType action,
		Upload? input)
	{
		if (action is not (ActionType.Read or ActionType.ReadAll) || input is null)
			return Task.CompletedTask;

		if (input.UserId is null
			|| _userAccessor.UserInRole(Roles.Admin)
			|| _userAccessor.IsSignedIn() && _userAccessor.GetUserId() == input.UserId
			|| !input.IsPrivate)
		{
			context.Approve();
		}

		return Task.CompletedTask;
	}
}