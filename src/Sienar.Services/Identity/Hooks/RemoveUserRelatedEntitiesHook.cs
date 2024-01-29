using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Services;

namespace Sienar.Identity.Hooks;

public class RemoveUserRelatedEntitiesHook : DbService<SienarUser>,
	IBeforeDelete<SienarUser>,
	IBeforeProcess<DeleteAccountRequest>
{
	private readonly IEntityDeleter<Medium> _mediaDeleter;
	private readonly IUserAccessor _userAccessor;

	/// <inheritdoc />
	public RemoveUserRelatedEntitiesHook(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IEntityDeleter<Medium> mediaDeleter,
		IUserAccessor userAccessor)
		: base(contextAccessor, logger, notifier)
	{
		_mediaDeleter = mediaDeleter;
		_userAccessor = userAccessor;
	}

	/// <inheritdoc />
	Task<HookStatus> IBeforeDelete<SienarUser>.Handle(SienarUser entity)
		=> HandleCore(entity);

	/// <inheritdoc />
	async Task<HookStatus> IBeforeProcess<DeleteAccountRequest>.Handle(DeleteAccountRequest request)
	{
		var userId = _userAccessor.GetUserId();
		if (!userId.HasValue)
		{
			Notifier.Error(ErrorMessages.Account.LoginRequired);
			return HookStatus.Unauthorized;
		}

		var user = await EntitySet.FindAsync(userId.Value);
		if (user is null)
		{
			Notifier.Error(ErrorMessages.Account.LoginRequired);
			return HookStatus.Unauthorized;
		}

		return await HandleCore(user);
	}

	private async Task<HookStatus> HandleCore(SienarUser entity)
	{
		await Context
			.Entry(entity)
			.Collection(u => u.VerificationCodes)
			.LoadAsync();
		await Context
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

		EntitySet.Update(entity);
		await Context.SaveChangesAsync();

		return HookStatus.Success;
	}
}