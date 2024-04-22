#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Errors;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;
using Sienar.Infrastructure.Services;

namespace Sienar.Identity.Processors;

/// <exclude />
public class UnlockUserAccountProcessor : DbService<SienarUser>,
	IProcessor<UnlockUserAccountRequest, bool>
{
	private readonly IUserManager _userManager;

	public UnlockUserAccountProcessor(
		DbContext context,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager)
		: base(context, logger, notifier)
	{
		_userManager = userManager;
	}

	public async Task<HookResult<bool>> Process(UnlockUserAccountRequest request)
	{
		var user = await _userManager.GetSienarUser(
			request.UserId,
			u => u.LockoutReasons);
		if (user is null)
		{
			return this.NotFound(message: CmsErrors.Account.NotFound);
		}

		user.LockoutEnd = null;
		user.LockoutReasons.Clear();

		EntitySet.Update(user);
		await Context.SaveChangesAsync();

		return this.Success(true, $"User {user.Username}'s account was unlocked successfully");
	}
}