using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

public class AccessTokenProcessor : IStatusProcessor<AccessTokenRequest>
{
	private readonly IHttpContextAccessor _accessor;

	public AccessTokenProcessor(IHttpContextAccessor accessor)
	{
		_accessor = accessor;
	}

	/// <inheritdoc />
	public async Task<OperationResult<bool>> Process(AccessTokenRequest request)
	{
		_accessor.HttpContext!.Response.Headers.Append(
			SienarConstants.AccessTokenHeader,
			"access-token-speegs92");

		return new();
	}
}