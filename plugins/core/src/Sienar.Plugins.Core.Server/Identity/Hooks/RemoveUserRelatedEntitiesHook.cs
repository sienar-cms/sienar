#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sienar.Identity.Requests;
using Sienar.Hooks;
using Sienar.Identity.Data;
using Sienar.Security;

namespace Sienar.Identity.Hooks;

/// <exclude />
public class RemoveUserRelatedEntitiesHook : IBeforeAction<SienarUser>,
	IBeforeAction<DeleteAccountRequest>
{
	private readonly IUserRepository _userRepository;
	private readonly IUserAccessor _userAccessor;

	public RemoveUserRelatedEntitiesHook(
		IUserRepository userRepository,
		IUserAccessor userAccessor)
	{
		_userRepository = userRepository;
		_userAccessor = userAccessor;
	}

	Task IBeforeAction<SienarUser>.Handle(
		SienarUser entity,
		ActionType action)
	{
		return action == ActionType.Delete
			? HandleCore(entity)
			: Task.CompletedTask;
	}

	async Task IBeforeAction<DeleteAccountRequest>.Handle(
		DeleteAccountRequest request,
		ActionType action)
	{
		if (action != ActionType.Status) return;

		var userId = (await _userAccessor.GetUserId())!;
		var user = (await _userRepository.Read(userId.Value))!;
		await HandleCore(user);
	}

	private async Task HandleCore(SienarUser entity)
	{
		await _userRepository.LoadVerificationCodes(entity);

		entity.VerificationCodes.Clear();

		await _userRepository.Update(entity);
	}
}