using System;
using System.Threading.Tasks;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Media.Hooks;

public class AssignMediaFieldsHook : IBeforeUpsert<Upload>
{
	private readonly IUserAccessor _userAccessor;

	public AssignMediaFieldsHook(IUserAccessor userAccessor)
	{
		_userAccessor = userAccessor;
	}

	/// <inheritdoc />
	public Task<HookStatus> Handle(Upload entity, bool isAdding)
	{
		var success = Task.FromResult(HookStatus.Success);

		// Only assign fields on create
		if (!isAdding) return success;

		if (!_userAccessor.UserInRole(Roles.Admin))
		{
			entity.UserId = _userAccessor.GetUserId();
		}

		entity.UploadedAt = DateTime.Now;

		return success;
	}
}