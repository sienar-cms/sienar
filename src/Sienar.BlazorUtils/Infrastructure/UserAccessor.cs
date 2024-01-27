using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Sienar.Infrastructure;

public class UserAccessor : IUserAccessor
{
	private readonly HttpContext _httpContext;

	public UserAccessor(IHttpContextAccessor httpContextAccessor)
	{
		_httpContext = httpContextAccessor.HttpContext!;
	}

	/// <inheritdoc />
	public bool IsSignedIn() => _httpContext.User.Identity?.IsAuthenticated ?? false;

	/// <inheritdoc />
	public Guid? GetUserId()
	{
		var claim = _httpContext.User.Claims
			.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
		return claim is null
			? null
			: Guid.Parse(claim.Value);
	}

	/// <inheritdoc />
	public string? GetUsername()
	{
		var claim = _httpContext.User.Claims
			.FirstOrDefault(c => c.Type == ClaimTypes.Name);
		return claim?.Value;
	}

	/// <inheritdoc />
	public ClaimsPrincipal GetUserClaimsPrincipal()
		=> _httpContext.User;

	/// <inheritdoc />
	public bool UserInRole(string roleName)
		=> _httpContext.User.IsInRole(roleName);
}