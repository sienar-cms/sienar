using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Sienar.Infrastructure.Hooks;
using Sienar.Utils;

namespace Sienar.Identity.Hooks;

public class UserPasswordUpdateHook : IBeforeUpsert<SienarUser>
{
	private readonly IPasswordHasher<SienarUser> _passwordHasher;

	public UserPasswordUpdateHook(IPasswordHasher<SienarUser> passwordHasher)
	{
		_passwordHasher = passwordHasher;
	}

	/// <inheritdoc />
	public Task<HookStatus> Handle(SienarUser user, bool isAdding)
	{
		if (user.Password != SienarConstants.PasswordPlaceholder)
		{
			user.PasswordHash = _passwordHasher.HashPassword(
				user,
				user.Password);
		}

		return Task.FromResult(HookStatus.Success);
	}
}