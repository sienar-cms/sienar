using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;

namespace Sienar.Identity;

public class BlazorServerSignInManager : IBlazorServerSignInManager
{
	protected const string LocalLoginDataKey = "Sienar.Login";

	protected readonly LoginOptions LoginOptions;
	protected readonly IUserManager UserManager;
	protected readonly AuthStateProvider AuthStateProvider;
	protected readonly AccountStateProvider AccountStateProvider;
	protected readonly ProtectedLocalStorage LocalStore;

	public BlazorServerSignInManager(
		IOptions<LoginOptions> loginOptions,
		IUserManager userManager,
		AuthStateProvider authStateProvider,
		AccountStateProvider accountStateProvider,
		ProtectedLocalStorage localStore)
	{
		LoginOptions = loginOptions.Value;
		UserManager = userManager;
		AuthStateProvider = authStateProvider;
		AccountStateProvider = accountStateProvider;
		LocalStore = localStore;
	}

	/// <inheritdoc />
	public virtual async Task SignIn(SienarUser user, bool isPersistent)
	{
		var loginData = CreateLoginData(user.Id, isPersistent, true);
		await LocalStore.SetAsync(LocalLoginDataKey, loginData);
		AccountStateProvider.User = user;
		AuthStateProvider.NotifyUserAuthentication(user);
	}

	public virtual async Task SignOut()
	{
		var loginData = CreateLoginData(Guid.Empty);
		await LocalStore.SetAsync(LocalLoginDataKey, loginData);
		AccountStateProvider.User = null;
		AuthStateProvider.NotifyUserAuthentication(null);
	}

	/// <inheritdoc />
	public async Task LoadUserLoginStatus()
	{
		ProtectedBrowserStorageResult<BlazorServerLoginData> dataRequest;
		try
		{
			dataRequest = await LocalStore.GetAsync<BlazorServerLoginData>(LocalLoginDataKey);
		}
		catch (Exception)
		{
			// Data was malformed
			await LocalStore.DeleteAsync(LocalLoginDataKey);
			return;
		}
		
		if (!dataRequest.Success)
		{
			return;
		}

		var loginData = dataRequest.Value!;
		if (!LoginValid(loginData))
		{
			await LocalStore.SetAsync(LocalLoginDataKey, CreateLoginData(Guid.Empty));
			AccountStateProvider.User = null;
			return;
		}

		var user = await UserManager.GetSienarUser(
			loginData.UserId,
			u => u.Roles);
		if (user is null)
		{
			await LocalStore.SetAsync(LocalLoginDataKey, CreateLoginData(Guid.Empty));
			AccountStateProvider.User = null;
			return;
		}

		AccountStateProvider.User = user;
		AuthStateProvider.NotifyUserAuthentication(user);
	}

	/// <inheritdoc />
	public async Task RefreshUserLoginStatus()
	{
		var dataRequest = await LocalStore.GetAsync<BlazorServerLoginData>(LocalLoginDataKey);
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
		await LocalStore.SetAsync(LocalLoginDataKey, newLoginData);
	}

	protected BlazorServerLoginData CreateLoginData(
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

	protected DateTimeOffset GetExpiration(bool isPersistent)
	{
		var duration = isPersistent
			? TimeSpan.FromDays(LoginOptions.PersistentLoginDuration)
			: TimeSpan.FromHours(LoginOptions.TransientLoginDuration);

		return DateTimeOffset.Now + duration;
	}

	protected bool LoginValid(BlazorServerLoginData data)
		=> data.IsAuthenticated && data.ExpiresAt > DateTimeOffset.Now;
}