#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Errors;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class PerformLoginProcessor : IProcessor<PerformLoginRequest, bool>
{
	private readonly ISignInManager _signInManager;
	private readonly IUserManager _userManager;
	private readonly LoginTokenCache _tokenCache;
	private readonly ILogger<PerformLoginProcessor> _logger;

	public PerformLoginProcessor(
		ISignInManager signInManager,
		IUserManager userManager,
		LoginTokenCache tokenCache,
		ILogger<PerformLoginProcessor> logger)
	{
		_signInManager = signInManager;
		_userManager = userManager;
		_tokenCache = tokenCache;
		_logger = logger;
	}

	public async Task<HookResult<bool>> Process(PerformLoginRequest request)
	{
		var loginRequest = _tokenCache.ConsumeLoginToken(request.LoginToken);
		if (loginRequest is null)
		{
			_logger.LogError("Login could not be performed: login request was null");
			return this.Unknown(message: CmsErrors.Account.LoginRequired);
		}

		var user = await _userManager.GetSienarUser(loginRequest.AccountName, u => u.Roles);
		if (user is null)
		{
			_logger.LogError("Login could not be performed: user does not exist");
			return this.Unknown(message: CmsErrors.Account.LoginRequired);
		}

		await _signInManager.SignIn(user, loginRequest.RememberMe);
		return this.Success(true);
	}
}