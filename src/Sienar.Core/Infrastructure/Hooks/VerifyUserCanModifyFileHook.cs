using System.Threading.Tasks;

namespace Sienar.Infrastructure.Hooks;

public class VerifyUserCanModifyFileHook
	: IBeforeUpsert<Medium>, IBeforeDelete<Medium>
{
	private readonly IUserAccessor _userAccessor;
	private readonly INotificationService _notifier;

	public VerifyUserCanModifyFileHook(IUserAccessor userAccessor, INotificationService notifier)
	{
		_userAccessor = userAccessor;
		_notifier = notifier;
	}

	/// <inheritdoc />
	Task<HookStatus> IBeforeUpsert<Medium>.Handle(Medium entity, bool isAdding)
	{
		// Only verify user can edit if actually editing
		if (isAdding) return Task.FromResult(HookStatus.Success);
		return CanModifyFile(entity);
	}

	/// <inheritdoc />
	Task<HookStatus> IBeforeDelete<Medium>.Handle(Medium entity)
		=> CanModifyFile(entity);

	private Task<HookStatus> CanModifyFile(Medium entity)
	{
		var success = Task.FromResult(HookStatus.Success);

		if (_userAccessor.UserInRole(Roles.Admin)) return success;
		if (_userAccessor.IsSignedIn()
		&& _userAccessor.GetUserId() == entity.UserId) return success;

		_notifier.Error("You do not have permission to modify another user's files");

		return Task.FromResult(HookStatus.Unauthorized);
	}
}