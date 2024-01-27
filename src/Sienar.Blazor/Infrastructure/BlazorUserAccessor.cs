using System;
using System.Linq;
using System.Security.Claims;
using Sienar.Identity;

namespace Sienar.Infrastructure;

public class BlazorUserAccessor : IUserAccessor
{
	private readonly AccountStateProvider _accountState;

	public BlazorUserAccessor(AccountStateProvider accountState)
	{
		_accountState = accountState;
	}

	/// <inheritdoc />
	public bool IsSignedIn() => _accountState.User is not null;

	/// <inheritdoc />
	public Guid? GetUserId() => _accountState.User?.Id;

	/// <inheritdoc />
	public string? GetUsername() => _accountState.User?.Username;

	/// <inheritdoc />
	public ClaimsPrincipal GetUserClaimsPrincipal() => throw new NotImplementedException();

	/// <inheritdoc />
	public bool UserInRole(string roleName) => _accountState.User?.Roles
		.FirstOrDefault(r => r.Name == roleName) is not null;
}