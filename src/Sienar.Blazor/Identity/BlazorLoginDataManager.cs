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

public class BlazorLoginDataManager : DbService<SienarUser>,
	IBlazorLoginDataManager
{
	private const string LocalLoginDataKey = "Sienar.Login";

	private readonly LoginOptions _loginOptions;
	private readonly ProtectedLocalStorage _localStore;

	/// <inheritdoc />
	public BlazorLoginDataManager(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IOptions<LoginOptions> loginOptions,
		ProtectedLocalStorage localStore)
		: base(contextAccessor, logger, notifier)
	{
		_loginOptions = loginOptions.Value;
		_localStore = localStore;
	}

	/// <inheritdoc />
	public ValueTask WriteLoginData(BlazorServerLoginData loginData)
		=> _localStore.SetAsync(LocalLoginDataKey, loginData);

	/// <inheritdoc />
	public ValueTask ClearLoginData()
	{
		var loginData = CreateLoginData(Guid.Empty);
		return _localStore.SetAsync(LocalLoginDataKey, loginData);
	}

	/// <inheritdoc />
	public BlazorServerLoginData CreateLoginData(
		Guid userId,
		bool isPersistent = false,
		bool isAuthenticated = false)
		=> new()
		{
			UserId = userId,
			ExpiresAt = GetExpiration(isPersistent),
			IsPersistent = isPersistent,
			IsAuthenticated = isAuthenticated
		};

	/// <inheritdoc />
	public async Task<SienarUser?> LoadUserLoginStatus()
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
			return null;
		}
		
		if (!dataRequest.Success)
		{
			return null;
		}

		var loginData = dataRequest.Value!;
		if (!LoginValid(loginData))
		{
			await ClearLoginData();
			return null;
		}

		var user = await EntitySet
			.AsNoTracking()
			.Include(u => u.Roles)
			.FirstOrDefaultAsync(u => u.Id == loginData.UserId);
		if (user is null)
		{
			Notifier.Error(ErrorMessages.Account.AccountDeleted);
			await ClearLoginData();
			return null;
		}

		if (user.IsLockedOut())
		{
			Notifier.Error(ErrorMessages.Account.AccountLocked);
			await ClearLoginData();
			return null;
		}

		return user;
		// _accountStateProvider.User = user;
		// _authStateProvider.NotifyUserAuthentication(user);
	}

	/// <inheritdoc />
	public async ValueTask RefreshUserLoginStatus()
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
			await ClearLoginData();
			return;
		}

		if (user.IsLockedOut())
		{
			Notifier.Error(ErrorMessages.Account.AccountLocked);
			await ClearLoginData();
			return;
		}

		var newLoginData = CreateLoginData(
			loginData.UserId,
			loginData.IsPersistent,
			loginData.IsAuthenticated);
		await _localStore.SetAsync(LocalLoginDataKey, newLoginData);
	}

	private DateTimeOffset GetExpiration(bool isPersistent)
	{
		var duration = isPersistent
			? TimeSpan.FromDays(_loginOptions.PersistentLoginDuration)
			: TimeSpan.FromHours(_loginOptions.TransientLoginDuration);

		return DateTimeOffset.Now + duration;
	}

	private static bool LoginValid(BlazorServerLoginData data)
		=> data.IsAuthenticated && data.ExpiresAt > DateTimeOffset.Now;
}