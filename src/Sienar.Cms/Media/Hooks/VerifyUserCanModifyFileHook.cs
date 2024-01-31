using System.Threading.Tasks;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Media.Hooks;

public class VerifyUserCanModifyFileHook
	: IBeforeUpsert<Upload>, IBeforeDelete<Upload>
{
	private readonly IUserAccessor _userAccessor;
	private readonly INotificationService _notifier;

	public VerifyUserCanModifyFileHook(IUserAccessor userAccessor, INotificationService notifier)
	{
		_userAccessor = userAccessor;
		_notifier = notifier;
	}

	/// <inheritdoc />
	Task<HookStatus> IBeforeUpsert<Upload>.Handle(Upload entity, bool isAdding)
	{
		// Only verify user can edit if actually editing
		if (isAdding) return Task.FromResult(HookStatus.Success);
		return CanModifyFile(entity);
	}

	/// <inheritdoc />
	Task<HookStatus> IBeforeDelete<Upload>.Handle(Upload entity)
		=> CanModifyFile(entity);

	private Task<HookStatus> CanModifyFile(Upload entity)
	{
		var success = Task.FromResult(HookStatus.Success);

		if (_userAccessor.UserInRole(Roles.Admin)) return success;
		if (_userAccessor.IsSignedIn()
		&& _userAccessor.GetUserId() == entity.UserId) return success;

		_notifier.Error("You do not have permission to modify another user's files");

		return Task.FromResult(HookStatus.Unauthorized);
	}
}