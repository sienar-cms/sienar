using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Identity.Hooks;

public class UserPasswordUpdateHook : IBeforeProcess<SienarUser>
{
	private readonly IPasswordHasher<SienarUser> _passwordHasher;

	public UserPasswordUpdateHook(IPasswordHasher<SienarUser> passwordHasher)
	{
		_passwordHasher = passwordHasher;
	}

	/// <inheritdoc />
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