#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Linq;
using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Hooks;
using Sienar.Identity.Results;
using Sienar.Infrastructure;
using Sienar.Processors;
using Sienar.Security;
using Sienar.Ui;

namespace Sienar.Identity.Processors;

/// <exclude />
public class LoadUserDataProcessor
	: IResultProcessor<AccountDataResult>, IBeforeTask<SienarStartupActor>
{
	private readonly IRestClient _client;
	private readonly IUserClaimsFactory _claimsFactory;
	private readonly AuthStateProvider _authStateProvider;

	public LoadUserDataProcessor(
		IRestClient client,
		IUserClaimsFactory claimsFactory,
		AuthStateProvider authStateProvider)
	{
		_client = client;
		_claimsFactory = claimsFactory;
		_authStateProvider = authStateProvider;
	}

	Task<OperationResult<AccountDataResult?>> IResultProcessor<AccountDataResult>.Process()
		=> LoadUserData();

	Task IBeforeTask<SienarStartupActor>.Handle(SienarStartupActor? a)
		=> LoadUserData();

	private async Task<OperationResult<AccountDataResult?>> LoadUserData()
	{
		var userResult = await _client.Get<AccountDataResult>("account");

		if (userResult.Status is not OperationStatus.Success)
		{
			return userResult;
		}

		var user = new SienarUser
		{
			Username = userResult.Result!.Username,
			Roles = userResult.Result!.Roles
				.Select(r => new SienarRole
				{
					Name = r
				})
				.ToList()
		};

		var userClaims = _claimsFactory.CreateClaims(user);
		_authStateProvider.NotifyUserAuthentication(userClaims, true);

		return userResult;
	}
}
