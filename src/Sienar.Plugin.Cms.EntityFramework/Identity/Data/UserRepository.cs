using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Data;
using Sienar.Processors;

namespace Sienar.Identity.Data;

public class UserRepository<TContext>
	: EntityFrameworkRepository<SienarUser, TContext>, IUserRepository
	where TContext : DbContext
{
	public UserRepository(
		TContext context,
		IFilterProcessor<SienarUser> filterProcessor)
		: base(context, filterProcessor) {}

	/// <inheritdoc />
	public async Task<SienarUser?> ReadUserByNameOrEmail(
		string name,
		Filter? filter = null)
	{
		Expression<Func<SienarUser, bool>> query = u => u.Username == name || u.Email == name;

		if (filter is null)
		{
			return await EntitySet.FirstOrDefaultAsync(query);
		}

		return await FilterProcessor
			.ProcessIncludes(EntitySet, filter)
			.FirstOrDefaultAsync(query);
	}

	/// <inheritdoc />
	public async Task<bool> UsernameIsTaken(Guid id, string username)
		=> await EntitySet
			.CountAsync(u => u.Id != id && u.Username == username) > 0;

	/// <inheritdoc />
	public async Task<bool> EmailIsTaken(Guid id, string email)
		=> await EntitySet
			.CountAsync(
				u => u.Id != id
					&& (u.Email == email || u.PendingEmail == email)) > 0;

	/// <inheritdoc />
	public async Task LoadVerificationCodes(SienarUser user)
		=> await Context
			.Entry(user)
			.Collection(u => u.VerificationCodes)
			.LoadAsync();

	/// <inheritdoc />
	public async Task LoadMedia(SienarUser user)
		=> await Context
			.Entry(user)
			.Collection(u => u.Media)
			.LoadAsync();
}