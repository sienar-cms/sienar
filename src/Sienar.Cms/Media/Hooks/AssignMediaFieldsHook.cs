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
	public Task Handle(Upload entity, ActionType action)
	{
		// Only assign fields on create
		if (action != ActionType.Create) return Task.CompletedTask;

		if (!_userAccessor.UserInRole(Roles.Admin))
		{
			entity.UserId = _userAccessor.GetUserId();
		}

		entity.UploadedAt = DateTime.Now;

		return Task.CompletedTask;
	}
}