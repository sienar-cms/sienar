using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Errors;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Services;

namespace Sienar.Identity;

public class BlazorServerSignInManager : DbService<SienarUser>,
	IBlazorServerSignInManager
{
	private const string LocalLoginDataKey = "Sienar.Login";

	private readonly LoginOptions _loginOptions;
	private readonly AuthStateProvider _authStateProvider;
	private readonly AccountStateProvider _accountStateProvider;
	private readonly ProtectedLocalStorage _localStore;

	public BlazorServerSignInManager(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<BlazorServerSignInManager> logger,
		INotificationService notifier,
		IOptions<LoginOptions> loginOptions,
		AuthStateProvider authStateProvider,
		AccountStateProvider accountStateProvider,
		ProtectedLocalStorage localStore)
		: base(contextAccessor, logger, notifier)
	{
		_loginOptions = loginOptions.Value;
		_authStateProvider = authStateProvider;
		_accountStateProvider = accountStateProvider;
		_localStore = localStore;
	}

	/// <inheritdoc />
	public async Task SignIn(SienarUser user, bool isPersistent)
	{
		var loginData = CreateLoginData(user.Id, isPersistent, true);
		await _localStore.SetAsync(LocalLoginDataKey, loginData);
		_accountStateProvider.User = user;
		_authStateProvider.NotifyUserAuthentication(user);
	}

	public async Task SignOut()
	{
		var loginData = CreateLoginData(Guid.Empty);
		await _localStore.SetAsync(LocalLoginDataKey, loginData);
		_accountStateProvider.User = null;
		_authStateProvider.NotifyUserAuthentication(null);
	}

	/// <inheritdoc />
	public async Task LoadUserLoginStatus()
	{
		ProtectedBrowserStorageResult<BlazorServerLoginData> dataRequest;
		try
		{
			dataRequest = await _localStore.GetAsync<BlazorServerLoginData>(LocalLoginDataKey);
		}
		catch (Exception)
		{
			// Data was malformed
			await _localStore.DeleteAsync(LocalLoginDataKey);
			return;
		}
		
		if (!dataRequest.Success)
		{
			return;
		}

		var loginData = dataRequest.Value!;
		if (!LoginValid(loginData))
		{
			await SignOut();
			return;
		}

		var user = await EntitySet
			.AsNoTracking()
			.Include(u => u.Roles)
			.FirstOrDefaultAsync(u => u.Id == loginData.UserId);
		if (user is null)
		{
			Notifier.Error(ErrorMessages.Account.AccountDeleted);
			await SignOut();
			return;
		}

		if (user.IsLockedOut())
		{
			Notifier.Error(ErrorMessages.Account.AccountLocked);
			await SignOut();
			return;
		}

		_accountStateProvider.User = user;
		_authStateProvider.NotifyUserAuthentication(user);
	}

	/// <inheritdoc />
	public async Task RefreshUserLoginStatus()
	{
		var dataRequest = await _localStore.GetAsync<BlazorServerLoginData>(LocalLoginDataKey);
		if (!dataRequest.Success)
		{
			return;
		}

		var loginData = dataRequest.Value!;
		if (!LoginValid(loginData))
		{
			return;
		}

		var user = await EntitySet
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Id == loginData.UserId);
		if (user is null)
		{
			Notifier.Error(ErrorMessages.Account.AccountDeleted);
			await SignOut();
			return;
		}

		if (user.IsLockedOut())
		{
			Notifier.Error(ErrorMessages.Account.AccountLocked);
			await SignOut();
			return;
		}

		var newLoginData = CreateLoginData(
			loginData.UserId,
			loginData.IsPersistent,
			loginData.IsAuthenticated);
		await _localStore.SetAsync(LocalLoginDataKey, newLoginData);
	}

	/// <inheritdoc />
	public async Task ForceSignOutIfCurrentUser(Guid id)
	{
		if (id == _accountStateProvider.User?.Id)
		{
			Notifier.Error("You have been signed out by an administrator");
			await SignOut();
		}
	}

	private BlazorServerLoginData CreateLoginData(
		Guid userId,
		bool isPersistent = false,
		bool isAuthenticated = false)
	{
		return new BlazorServerLoginData
		{
			UserId = userId,
			ExpiresAt = GetExpiration(isPersistent),
			IsPersistent = isPersistent,
			IsAuthenticated = isAuthenticated
		};
	}

	private DateTimeOffset GetExpiration(bool isPersistent)
	{
		var duration = isPersistent
			? TimeSpan.FromDays(_loginOptions.PersistentLoginDuration)
			: TimeSpan.FromHours(_loginOptions.TransientLoginDuration);

		return DateTimeOffset.Now + duration;
	}

	private bool LoginValid(BlazorServerLoginData data)
		=> data.IsAuthenticated && data.ExpiresAt > DateTimeOffset.Now;
}