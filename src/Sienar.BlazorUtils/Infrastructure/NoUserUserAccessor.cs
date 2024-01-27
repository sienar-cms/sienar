using System;
using System.Security.Claims;

namespace Sienar.Infrastructure;

public class NoUserUserAccessor : IUserAccessor
{
	/// <inheritdoc />
	public bool IsSignedIn() => true;

	/// <inheritdoc />
	public Guid? GetUserId() => null;

	/// <inheritdoc />
	public string? GetUsername() => null;

	/// <inheritdoc />
	public ClaimsPrincipal GetUserClaimsPrincipal() => new();

	/// <inheritdoc />
	public bool UserInRole(string roleName) => true;
}