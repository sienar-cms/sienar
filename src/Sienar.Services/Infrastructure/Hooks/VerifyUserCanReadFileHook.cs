using System.Threading.Tasks;

namespace Sienar.Infrastructure.Hooks;

public class VerifyUserCanReadFileHook : IAfterRead<Medium>
{
	private readonly IUserAccessor _userAccessor;
	private readonly INotificationService _notifier;

	public VerifyUserCanReadFileHook(IUserAccessor userAccessor, INotificationService notifier)
	{
		_userAccessor = userAccessor;
		_notifier = notifier;
	}

	/// <inheritdoc />
	public Task<HookStatus> Handle(Medium entity, bool isSingle)
	{
		var success = Task.FromResult(HookStatus.Success);

		if (entity.UserId is null) return success;
		if (_userAccessor.UserInRole(Roles.Admin)) return success;
		if (_userAccessor.GetUserId() == entity.UserId) return success;
		if (!entity.IsPrivate) return success;

		_notifier.Error($"You do not have permission to access the file {entity.Title}");

		return Task.FromResult(HookStatus.Unauthorized);
	}
}