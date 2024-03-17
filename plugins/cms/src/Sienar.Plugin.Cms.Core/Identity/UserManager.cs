using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Sienar.Identity;

public class UserManager : IUserManager
{
	private readonly DbContext _context;
	private readonly IPasswordHasher<SienarUser> _passwordHasher;
	private DbSet<SienarUser> UserSet => _context.Set<SienarUser>();

	public UserManager(
		DbContext context,
		IPasswordHasher<SienarUser> passwordHasher)
	{
		_context = context;
		_passwordHasher = passwordHasher;
	}

	/// <inheritdoc />
	public Task<SienarUser?> GetSienarUser(ClaimsPrincipal claims)
		=> GetSienarUser<bool>(claims, null);

	/// <inheritdoc />
	public async Task<SienarUser?> GetSienarUser<TProperty>(
		ClaimsPrincipal claims,
		Expression<Func<SienarUser, TProperty>>? include)
	{
		var id = claims.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
		if (id is null)
		{
			return null;
		}

		var guidId = Guid.Parse(id.Value);
		return await GetSienarUser(u => u.Id == guidId, include);
	}

	/// <inheritdoc />
	public Task<SienarUser?> GetSienarUser(Guid id)
		=> GetSienarUser<bool>(id, null);

	/// <inheritdoc />
	public Task<SienarUser?> GetSienarUser<TProperty>(
		Guid id,
		Expression<Func<SienarUser, TProperty>>? include)
		=> GetSienarUser(u => u.Id == id, include);

	/// <inheritdoc />
	public Task<SienarUser?> GetSienarUser(string name)
		=> GetSienarUser<bool>(name, null);

	/// <inheritdoc />
	public async Task<SienarUser?> GetSienarUser<TProperty>(
		string name,
		Expression<Func<SienarUser, TProperty>>? include)
		=> await GetSienarUser(
			u => u.Username == name || u.Email == name,
			include);

	/// <inheritdoc />
	public async Task UpdatePassword(
		SienarUser user,
		string newPassword)
	{
		user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
		UserSet.Update(user);
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
			UserSet.Update(user);
			await _context.SaveChangesAsync();
		}

		return true;
	}

	/// <inheritdoc />
	public async Task DeleteUser(ClaimsPrincipal claims)
	{
		var user = await GetSienarUser(claims);
		if (user is null)
		{
			return;
		}

		UserSet.Remove(user);
		await _context.SaveChangesAsync();
	}

	/// <summary>
	/// Gets a single user if it exists based on the given predicate. Optionally includes relations.
	/// </summary>
	/// <param name="where">The predicate to search for the single user by</param>
	/// <param name="include">An expression to pass to the <c>IQueryable&lt;SienarUser&gt;.Include()</c> method</param>
	/// <returns></returns>
	private async Task<SienarUser?> GetSienarUser<TProperty>(
		Expression<Func<SienarUser, bool>> where,
		Expression<Func<SienarUser, TProperty>>? include)
	{
		IQueryable<SienarUser> users = UserSet;
		if (include is not null)
		{
			users = users.Include(include);
		}
		return await users.FirstOrDefaultAsync(where);
	}
}