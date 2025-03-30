#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Sienar.Identity;

/// <exclude />
public class PasswordManager<TContext> : IPasswordManager
	where TContext : DbContext
{
	private readonly IPasswordHasher<SienarUser> _passwordHasher;
	private readonly TContext _context;

	public PasswordManager(
		IPasswordHasher<SienarUser> passwordHasher,
		TContext context)
	{
		_passwordHasher = passwordHasher;
		_context = context;
	}

	/// <inheritdoc />
	public async Task UpdatePassword(
		SienarUser user,
		string newPassword)
	{
		user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
		_context.Update(user);
		await _context.SaveChangesAsync();
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
			_context.Update(user);
			await _context.SaveChangesAsync();
		}

		return true;
	}
}