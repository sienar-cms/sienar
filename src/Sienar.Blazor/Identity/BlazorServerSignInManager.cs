using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;

namespace Sienar.Identity;

public class BlazorServerSignInManager : IBlazorServerSignInManager
{
	private const string LocalLoginDataKey = "Sienar.Login";

	private readonly LoginOptions _loginOptions;
	private readonly IUserManager _userManager;
	private readonly AuthStateProvider _authStateProvider;
	private readonly AccountStateProvider _accountStateProvider;
	private readonly ProtectedLocalStorage _localStore;

	public BlazorServerSignInManager(
		IOptions<LoginOptions> loginOptions,
		IUserManager userManager,
		AuthStateProvider authStateProvider,
		AccountStateProvider accountStateProvider,
		ProtectedLocalStorage localStore)
	{
		_loginOptions = loginOptions.Value;
		_userManager = userManager;
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
			await _localStore.SetAsync(LocalLoginDataKey, CreateLoginData(Guid.Empty));
			_accountStateProvider.User = null;
			return;
		}

		var user = await _userManager.GetSienarUser(
			loginData.UserId,
			u => u.Roles);
		if (user is null)
		{
			await _localStore.SetAsync(LocalLoginDataKey, CreateLoginData(Guid.Empty));
			_accountStateProvider.User = null;
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

		var newLoginData = CreateLoginData(
			loginData.UserId,
			loginData.IsPersistent,
			loginData.IsAuthenticated);
		await _localStore.SetAsync(LocalLoginDataKey, newLoginData);
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