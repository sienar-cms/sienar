using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sienar.Infrastructure;

namespace Sienar.Identity;

public class UserManager : IUserManager
{
	protected readonly IDbContextAccessor<DbContext> ContextAccessor;
	protected readonly IPasswordHasher<SienarUser> PasswordHasher;
	protected DbContext Context => ContextAccessor.Context;
	protected DbSet<SienarUser> UserSet => Context.Set<SienarUser>();

	public UserManager(
		IDbContextAccessor<DbContext> contextAccessor,
		IPasswordHasher<SienarUser> passwordHasher)
	{
		ContextAccessor = contextAccessor;
		PasswordHasher = passwordHasher;
	}

	/// <inheritdoc />
	public virtual Task<SienarUser?> GetSienarUser(ClaimsPrincipal claims)
		=> GetSienarUser<bool>(claims, null);

	/// <inheritdoc />
	public virtual async Task<SienarUser?> GetSienarUser<TProperty>(
		ClaimsPrincipal claims,
		Expression<Func<SienarUser, TProperty>>? include)
	{
		var id = claims.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
		if (id is null)
		{
			return null;
		}

		var guidId = Guid.Parse(id.Value);
		return await GeSienarUser(u => u.Id == guidId, include);
	}

	/// <inheritdoc />
	public virtual Task<SienarUser?> GetSienarUser(Guid id)
		=> GetSienarUser<bool>(id, null);

	/// <inheritdoc />
	public virtual Task<SienarUser?> GetSienarUser<TProperty>(
		Guid id,
		Expression<Func<SienarUser, TProperty>>? include)
		=> GeSienarUser(u => u.Id == id, include);

	/// <inheritdoc />
	public virtual Task<SienarUser?> GetSienarUser(string name)
		=> GetSienarUser<bool>(name, null);

	/// <inheritdoc />
	public virtual async Task<SienarUser?> GetSienarUser<TProperty>(
		string name,
		Expression<Func<SienarUser, TProperty>>? include)
		=> await GeSienarUser(
			u => u.Username == name || u.Email == name,
			include);

	/// <inheritdoc />
	public virtual async Task UpdatePassword(
		SienarUser user,
		string newPassword)
	{
		user.PasswordHash = PasswordHasher.HashPassword(user, newPassword);
		UserSet.Update(user);
		await Context.SaveChangesAsync();
	}

	/// <inheritdoc />
	public virtual async Task<bool> VerifyPassword(SienarUser user, string password)
	{
		var verification = PasswordHasher.VerifyHashedPassword(
			user,
			user.PasswordHash,
			password);

		if (verification == PasswordVerificationResult.Failed)
		{
			return false;
		}

		if (verification == PasswordVerificationResult.SuccessRehashNeeded)
		{
			user.PasswordHash = PasswordHasher.HashPassword(user, password);
			UserSet.Update(user);
			await Context.SaveChangesAsync();
		}

		return true;
	}

	/// <inheritdoc />
	public virtual async Task DeleteUser(ClaimsPrincipal claims)
	{
		var user = await GetSienarUser(claims);
		if (user is null)
		{
			return;
		}

		UserSet.Remove(user);
		await Context.SaveChangesAsync();
	}

	/// <summary>
	/// Gets a single user if it exists based on the given predicate. Optionally includes relations.
	/// </summary>
	/// <param name="where">The predicate to search for the single user by</param>
	/// <returns></returns>
	protected async Task<SienarUser?> GeSienarUser(Expression<Func<SienarUser, bool>> where)
		=> await UserSet.FirstOrDefaultAsync(where);

	/// <summary>
	/// Gets a single user if it exists based on the given predicate. Optionally includes relations.
	/// </summary>
	/// <param name="where">The predicate to search for the single user by</param>
	/// <param name="include">An expression to pass to the <c>IQueryable&lt;SienarUser&gt;.Include()</c> method</param>
	/// <returns></returns>
	protected async Task<SienarUser?> GeSienarUser<TProperty>(
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