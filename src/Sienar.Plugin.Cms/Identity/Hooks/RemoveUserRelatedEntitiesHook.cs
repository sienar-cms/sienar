#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Data;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Hooks;
using Sienar.Identity.Data;
using Sienar.Services;
using Sienar.Media;

namespace Sienar.Identity.Hooks;

/// <exclude />
public class RemoveUserRelatedEntitiesHook<TContext> :
	IBeforeAction<SienarUser>,
	IBeforeAction<DeleteAccountRequest>
	where TContext : DbContext
{
	private readonly TContext _context;
	private readonly IEntityDeleter<Upload> _mediaDeleter;
	private readonly IUserAccessor _userAccessor;

	public RemoveUserRelatedEntitiesHook(
		TContext context,
		IEntityDeleter<Upload> mediaDeleter,
		IUserAccessor userAccessor)
	{
		_context = context;
		_mediaDeleter = mediaDeleter;
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
		if (action != ActionType.StatusAction) return;

		var userId = (await _userAccessor.GetUserId())!;
		var user = (await _context.FindAsync<SienarUser>(userId.Value))!;
		await HandleCore(user);
	}

	private async Task HandleCore(SienarUser entity)
	{
		await _context
			.Entry(entity)
			.Collection(u => u.VerificationCodes)
			.LoadAsync();
		await _context
			.Entry(entity)
			.Collection(u => u.Media)
			.LoadAsync();

		entity.VerificationCodes.Clear();

		var filesToDelete = new List<Guid>();

		foreach (var media in entity.Media)
		{
			if (media.IsPrivate)
			{
				filesToDelete.Add(media.Id);
			}
			else
			{
				media.UserId = null;
			}
		}

		entity.Media.Clear();

		foreach (var id in filesToDelete)
		{
			await _mediaDeleter.Delete(id);
		}

		_context.Update(entity);
		await _context.SaveChangesAsync();
	}
}