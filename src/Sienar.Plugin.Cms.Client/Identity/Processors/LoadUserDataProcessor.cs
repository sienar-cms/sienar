#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Components;
using Sienar.Data;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class LoadUserDataProcessor
	: IProcessor<SienarUser>, IBeforeTask<SienarStartupActor>
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

	Task<OperationResult<SienarUser?>> IProcessor<SienarUser>.Process()
		=> LoadUserData();

	Task IBeforeTask<SienarStartupActor>.Handle(SienarStartupActor? a)
		=> LoadUserData();

	private async Task<OperationResult<SienarUser?>> LoadUserData()
	{
		var user = (await _client.Get<SienarUser>("account")).Result;

		if (user is null)
		{
			return new OperationResult<SienarUser?>(OperationStatus.Unauthorized);
		}

		var userClaims = _claimsFactory.CreateClaims(user);
		_authStateProvider.NotifyUserAuthentication(userClaims, true);
		return new OperationResult<SienarUser?>();
	}
}
