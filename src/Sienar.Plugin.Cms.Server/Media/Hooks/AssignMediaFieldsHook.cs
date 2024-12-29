using System;
using System.Threading.Tasks;
using Sienar.Infrastructure;
using Sienar.Hooks;

namespace Sienar.Media.Hooks;

public class AssignMediaFieldsHook : IBeforeProcess<Upload>
{
	private readonly IUserAccessor _userAccessor;

	public AssignMediaFieldsHook(IUserAccessor userAccessor)
	{
		_userAccessor = userAccessor;
	}

	/// <inheritdoc />
	public async Task Handle(Upload entity, ActionType action)
	{
		// Only assign fields on create
		if (action != ActionType.Create) return;

		if (!await _userAccessor.UserInRole(Roles.Admin))
		{
			entity.UserId = await _userAccessor.GetUserId();
		}

		entity.UploadedAt = DateTime.UtcNow;
	}
}