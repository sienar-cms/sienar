using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Identity.Requests;
using Sienar.Identity.Results;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

public class ClientAccountLockoutProcessor : IProcessor<AccountLockoutRequest, AccountLockoutResult>
{
	private readonly IRestClient _client;

	public ClientAccountLockoutProcessor(IRestClient client)
	{
		_client = client;
	}

	public Task<OperationResult<AccountLockoutResult?>> Process(AccountLockoutRequest request)
		=> _client.Get<AccountLockoutResult>("account/lockout-reasons", request);
}
