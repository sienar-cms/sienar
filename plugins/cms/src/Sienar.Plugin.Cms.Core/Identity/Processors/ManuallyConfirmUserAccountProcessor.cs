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
public class ManuallyConfirmUserAccountProcessor : DbService<SienarUser>,
	IProcessor<ManuallyConfirmUserAccountRequest, bool>
{
	private readonly IUserManager _userManager;

	public ManuallyConfirmUserAccountProcessor(
		DbContext context,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager)
		: base(context, logger, notifier)
	{
		_userManager = userManager;
	}

	public async Task<HookResult<bool>> Process(ManuallyConfirmUserAccountRequest request)
	{
		var user = await _userManager.GetSienarUser(request.UserId);
		if (user is null)
		{
			return this.NotFound(message: CmsErrors.Account.NotFound);
		}

		if (user.EmailConfirmed)
		{
			return this.Unprocessable(message: $"{user.Username}'s account is already confirmed");
		}

		user.EmailConfirmed = true;

		EntitySet.Update(user);
		await Context.SaveChangesAsync();

		return this.Success(true, $"Confirmed {user.Username}'s account");
	}
}