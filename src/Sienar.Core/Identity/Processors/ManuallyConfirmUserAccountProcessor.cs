using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;
using Sienar.Infrastructure.Services;

namespace Sienar.Identity.Processors;

public class ManuallyConfirmUserAccountProcessor : DbService<SienarUser>,
	IProcessor<ManuallyConfirmUserAccountRequest>
{
	private readonly IUserManager _userManager;
	private SienarUser? _user;

	/// <inheritdoc />
	public ManuallyConfirmUserAccountProcessor(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager)
		: base(contextAccessor, logger, notifier)
	{
		_userManager = userManager;
	}

	/// <inheritdoc />
	public async Task<HookStatus> Process(ManuallyConfirmUserAccountRequest request)
	{
		_user = await _userManager.GetSienarUser(request.UserId);
		if (_user is null)
		{
			Notifier.Error(ErrorMessages.Account.NotFound);
			return HookStatus.NotFound;
		}

		if (_user.EmailConfirmed)
		{
			Notifier.Warning($"{_user.Username}'s account is already confirmed");
			return HookStatus.Unprocessable;
		}

		_user.EmailConfirmed = true;

		EntitySet.Update(_user);
		await Context.SaveChangesAsync();

		return HookStatus.Success;
	}

	/// <inheritdoc />
	public void NotifySuccess()
	{
		Notifier.Success($"Confirmed {_user?.Username}'s account");
	}

	/// <inheritdoc />
	public void NotifyFailure()
	{
		Notifier.Error("Unable to confirm account");
	}

	/// <inheritdoc />
	public void NotifyNoPermission()
	{
		Notifier.Error("You do not have permission to manually confirm accounts");
	}
}