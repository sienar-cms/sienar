#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Identity.Results;
using Sienar.Processors;
using Sienar.Security;

namespace Sienar.Identity.Processors;

/// <exclude />
public class GetAccountDataProcessor : IResultProcessor<AccountDataResult>
{
	private readonly IUserAccessor _userAccessor;

	public GetAccountDataProcessor(IUserAccessor userAccessor)
	{
		_userAccessor = userAccessor;
	}

	public async Task<OperationResult<AccountDataResult?>> Process()
	{
		if (!await _userAccessor.IsSignedIn())
		{
			return new(OperationStatus.Unauthorized);
		}

		var roles = (await _userAccessor.GetUserClaimsPrincipal()).Claims
			.Where(c => c.Type == ClaimTypes.Role)
			.Select(c => c.Value)
			.ToArray();

		var result = new AccountDataResult
		{
			Username = (await _userAccessor.GetUsername())!,
			Roles = roles
		};

		return new(result: result);
	}
}
