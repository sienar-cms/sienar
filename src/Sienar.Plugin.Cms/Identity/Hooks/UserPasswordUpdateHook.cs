#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Sienar.Hooks;

namespace Sienar.Identity.Hooks;

/// <exclude />
public class UserPasswordUpdateHook : IBeforeAction<SienarUser>
{
	private readonly IPasswordHasher<SienarUser> _passwordHasher;

	public UserPasswordUpdateHook(IPasswordHasher<SienarUser> passwordHasher)
	{
		_passwordHasher = passwordHasher;
	}

	public Task Handle(SienarUser user, ActionType action)
	{
		if (action is not (ActionType.Create or ActionType.Update)) return Task.CompletedTask;

		if (user.Password != SienarConstants.PasswordPlaceholder)
		{
			user.PasswordHash = _passwordHasher.HashPassword(
				user,
				user.Password);
		}

		return Task.CompletedTask;
	}
}