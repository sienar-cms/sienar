using System.Threading.Tasks;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

public class PerformLoginProcessor : IProcessor<PerformLoginRequest, bool>
{
	private readonly ISignInManager _signInManager;
	private readonly IUserManager _userManager;
	private readonly LoginTokenCache _tokenCache;
	private readonly INotificationService _notifier;

	public PerformLoginProcessor(
		ISignInManager signInManager,
		IUserManager userManager,
		LoginTokenCache tokenCache,
		INotificationService notifier)
	{
		_signInManager = signInManager;
		_userManager = userManager;
		_tokenCache = tokenCache;
		_notifier = notifier;
	}

	/// <inheritdoc />
	public async Task<HookResult<bool>> Process(PerformLoginRequest request)
	{
		var loginRequest = _tokenCache.ConsumeLoginToken(request.LoginToken);
		if (loginRequest is null)
		{
			_notifier.Error("Your login could not be completed: error code LOGIN-1");
			return this.Unknown();
		}

		var user = await _userManager.GetSienarUser(loginRequest.AccountName, u => u.Roles);
		if (user is null)
		{
			_notifier.Error("Your login could not be completed: error code LOGIN-2");
			return this.Unknown();
		}

		await _signInManager.SignIn(user, loginRequest.RememberMe);
		return this.Success(true);
	}

	// Notifiers are blank on purpose. The user should already have been notified of their login success, and the user will be notified manually if there are problems with the login process from here because there *shouldn't* be any issues at this point

	/// <inheritdoc />
	public void NotifySuccess() {}

	/// <inheritdoc />
	public void NotifyFailure() {}

	/// <inheritdoc />
	public void NotifyNoPermission() {}
}