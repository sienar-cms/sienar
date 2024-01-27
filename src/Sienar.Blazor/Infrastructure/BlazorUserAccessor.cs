using System;
using System.Linq;
using System.Security.Claims;
using Sienar.Identity;

namespace Sienar.Infrastructure;

public class BlazorUserAccessor : IUserAccessor
{
	protected readonly AccountStateProvider AccountState;

	public BlazorUserAccessor(AccountStateProvider accountState)
	{
		AccountState = accountState;
	}

	/// <inheritdoc />
	public bool IsSignedIn() => AccountState.User is not null;

	/// <inheritdoc />
	public Guid? GetUserId() => AccountState.User?.Id;

	/// <inheritdoc />
	public string? GetUsername() => AccountState.User?.Username;

	/// <inheritdoc />
	public ClaimsPrincipal GetUserClaimsPrincipal() => throw new NotImplementedException();

	/// <inheritdoc />
	public bool UserInRole(string roleName) => AccountState.User?.Roles
		.FirstOrDefault(r => r.Name == roleName) is not null;
}