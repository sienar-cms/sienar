using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sienar.Data;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

public class AccessTokenProcessor : IProcessor<AccessTokenRequest, bool>
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