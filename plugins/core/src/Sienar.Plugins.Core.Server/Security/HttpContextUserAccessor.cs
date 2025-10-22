using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Sienar.Security;

public class HttpContextUserAccessor : IUserAccessor
{
	protected readonly HttpContext HttpContext;

	public HttpContextUserAccessor(IHttpContextAccessor httpContextAccessor)
	{
		HttpContext = httpContextAccessor.HttpContext!;
	}

	/// <inheritdoc />
	public Task<bool> IsSignedIn() => Task.FromResult(
		HttpContext.User.Identity?.IsAuthenticated ?? false);

	/// <inheritdoc />
	public virtual Task<Guid?> GetUserId()
	{
		var claim = HttpContext.User.Claims
			.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
		Guid? id = claim is null
			? null
			: Guid.Parse(claim.Value);
		return Task.FromResult(id);
	}

	/// <inheritdoc />
	public virtual Task<string?> GetUsername()
	{
		var claim = HttpContext.User.Claims
			.FirstOrDefault(c => c.Type == ClaimTypes.Name);
		return Task.FromResult(claim?.Value);
	}

	/// <inheritdoc />
	public virtual Task<ClaimsPrincipal> GetUserClaimsPrincipal()
		=> Task.FromResult(HttpContext.User);

	/// <inheritdoc />
	public Task<bool> UserInRole(string roleName)
		=> Task.FromResult(HttpContext.User.IsInRole(roleName));
}