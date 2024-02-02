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
	public Task<HookStatus> Handle(SienarUser user, ActionType action)
	{
		var success = Task.FromResult(HookStatus.Success);
		if (action is not (ActionType.Create or ActionType.Update)) return success;

		if (user.Password != SienarConstants.PasswordPlaceholder)
		{
			user.PasswordHash = _passwordHasher.HashPassword(
				user,
				user.Password);
		}

		return success;
	}
}