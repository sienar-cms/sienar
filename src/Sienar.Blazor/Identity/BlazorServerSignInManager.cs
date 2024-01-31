using System;
using System.Threading.Tasks;
using Sienar.Infrastructure;

namespace Sienar.Identity;

public class BlazorServerSignInManager : IBlazorServerSignInManager
{
	private readonly AuthStateProvider _authStateProvider;
	private readonly AccountStateProvider _accountStateProvider;
	private readonly IBlazorLoginDataManager _loginDataManager;
	private readonly INotificationService _notifier;

	public BlazorServerSignInManager(
		AuthStateProvider authStateProvider,
		AccountStateProvider accountStateProvider,
		IBlazorLoginDataManager loginDataManager,
		INotificationService notifier)
	{
		_authStateProvider = authStateProvider;
		_accountStateProvider = accountStateProvider;
		_loginDataManager = loginDataManager;
		_notifier = notifier;
	}

	/// <inheritdoc />
	public async Task SignIn(SienarUser user, bool isPersistent)
	{
		var loginData = _loginDataManager.CreateLoginData(user.Id, isPersistent, true);
		await _loginDataManager.WriteLoginData(loginData);
		_accountStateProvider.User = user;
		_authStateProvider.NotifyUserAuthentication(user);
	}

	public async Task SignOut()
	{
		await _loginDataManager.ClearLoginData();
		_accountStateProvider.User = null;
		_authStateProvider.NotifyUserAuthentication(null);
	}

	/// <inheritdoc />
	public async Task ForceSignOutIfCurrentUser(Guid id)
	{
		if (id == _accountStateProvider.User?.Id)
		{
			_notifier.Error("You have been signed out by an administrator");
			await SignOut();
		}
	}
}