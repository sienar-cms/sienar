using System;
using System.Threading.Tasks;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Media.Hooks;

public class AssignMediaFieldsHook : IBeforeProcess<Upload>
{
	private readonly IUserAccessor _userAccessor;

	public AssignMediaFieldsHook(IUserAccessor userAccessor)
	{
		_userAccessor = userAccessor;
	}

	/// <inheritdoc />
	public Task<HookStatus> Handle(Upload entity, ActionType action)
	{
		var success = Task.FromResult(HookStatus.Success);

		// Only assign fields on create
		if (action != ActionType.Create) return success;

		if (!_userAccessor.UserInRole(Roles.Admin))
		{
			entity.UserId = _userAccessor.GetUserId();
		}

		entity.UploadedAt = DateTime.Now;

		return success;
	}
}