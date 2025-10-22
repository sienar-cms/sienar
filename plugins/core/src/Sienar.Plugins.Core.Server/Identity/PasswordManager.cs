#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Sienar.Identity.Data;

namespace Sienar.Identity;

/// <exclude />
public class PasswordManager : IPasswordManager
{
	private readonly IPasswordHasher<SienarUser> _passwordHasher;
	private readonly IUserRepository _userRepository;

	public PasswordManager(
		IPasswordHasher<SienarUser> passwordHasher,
		IUserRepository userRepository)
	{
		_passwordHasher = passwordHasher;
		_userRepository = userRepository;
	}

	/// <inheritdoc />
	public async Task UpdatePassword(
		SienarUser user,
		string newPassword)
	{
		user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
		await _userRepository.Update(user);
	}

	/// <inheritdoc />
	public async Task<bool> VerifyPassword(SienarUser user, string password)
	{
		var verification = _passwordHasher.VerifyHashedPassword(
			user,
			user.PasswordHash,
			password);

		if (verification == PasswordVerificationResult.Failed)
		{
			return false;
		}

		if (verification == PasswordVerificationResult.SuccessRehashNeeded)
		{
			user.PasswordHash = _passwordHasher.HashPassword(user, password);
			await _userRepository.Update(user);
		}

		return true;
	}
}