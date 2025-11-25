#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class UnlockUserAccountProcessor<TContext> : IStatusProcessor<UnlockUserAccountRequest>
	where TContext : DbContext
{
	private readonly TContext _context;

	public UnlockUserAccountProcessor(TContext context)
	{
		_context = context;
	}

	public async Task<OperationResult<bool>> Process(UnlockUserAccountRequest request)
	{
		var userSet = _context.Set<SienarUser>();
		var user = await userSet
			.Include(u => u.LockoutReasons)
			.FirstOrDefaultAsync(u => u.Id == request.UserId);
		if (user is null)
		{
			return new(
				OperationStatus.NotFound,
				message: CoreErrors.Account.NotFound);
		}

		user.LockoutEnd = null;
		user.LockoutReasons.Clear();

		userSet.Update(user);
		await _context.SaveChangesAsync();

	return new(
		OperationStatus.Success,
		true,
		$"User {user.Username}'s account was unlocked successfully");
	}
}