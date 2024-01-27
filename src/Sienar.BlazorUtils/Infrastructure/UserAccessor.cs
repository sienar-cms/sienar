using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Sienar.Infrastructure;

public class UserAccessor : IUserAccessor
{
	protected readonly HttpContext HttpContext;

	public UserAccessor(IHttpContextAccessor httpContextAccessor)
	{
		HttpContext = httpContextAccessor.HttpContext!;
	}

	/// <inheritdoc />
	public bool IsSignedIn() => HttpContext.User.Identity?.IsAuthenticated ?? false;

	/// <inheritdoc />
	public virtual Guid? GetUserId()
	{
		var claim = HttpContext.User.Claims
			.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
		return claim is null
			? null
			: Guid.Parse(claim.Value);
	}

	/// <inheritdoc />
	public virtual string? GetUsername()
	{
		var claim = HttpContext.User.Claims
			.FirstOrDefault(c => c.Type == ClaimTypes.Name);
		return claim?.Value;
	}

	/// <inheritdoc />
	public virtual ClaimsPrincipal GetUserClaimsPrincipal()
		=> HttpContext.User;

	/// <inheritdoc />
	public bool UserInRole(string roleName)
		=> HttpContext.User.IsInRole(roleName);
}