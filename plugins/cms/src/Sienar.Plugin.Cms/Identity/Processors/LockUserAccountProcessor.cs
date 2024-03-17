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

public class LockUserAccountProcessor : DbService<SienarUser>,
	IProcessor<LockUserAccountRequest, bool>
{
	private readonly IUserManager _userManager;
	private SienarUser? _user;

	/// <inheritdoc />
	public LockUserAccountProcessor(
		DbContext context,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager)
		: base(context, logger, notifier)
	{
		_userManager = userManager;	
	}

	/// <inheritdoc />
	public async Task<HookResult<bool>> Process(LockUserAccountRequest request)
	{
		_user = await _userManager.GetSienarUser(
			request.UserId,
			u => u.LockoutReasons);

		if (_user is null)
		{
			Notifier.Error(ErrorMessages.Account.NotFound);
			return this.NotFound();
		}

		foreach (var id in request.Reasons)
		{
			var reason = await Context
				.Set<LockoutReason>()
				.FindAsync(id);
			if (reason is null)
			{
				Notifier.Error(ErrorMessages.LockoutReason.NotFound);
				return this.NotFound();
			}

			_user.LockoutReasons.Add(reason);
		}

		_user.LockoutEnd = request.EndDate;

		EntitySet.Update(_user);
		await Context.SaveChangesAsync();

		return this.Success(true);
	}

	/// <inheritdoc />
	public void NotifySuccess()
	{
		Notifier.Success($"Locked user {_user?.Username} successfully");
	}

	/// <inheritdoc />
	public void NotifyFailure()
	{
		Notifier.Error("Failed to lock user account");
	}

	/// <inheritdoc />
	public void NotifyNoPermission()
	{
		Notifier.Error("You do not have permission to lock user accounts");
	}
}