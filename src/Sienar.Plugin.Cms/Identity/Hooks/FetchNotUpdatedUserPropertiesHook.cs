using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Hooks;

namespace Sienar.Identity.Hooks;

public class FetchNotUpdatedUserPropertiesHook<TContext> : IBeforeAction<SienarUser>
	where TContext : DbContext
{
	private readonly TContext _context;

	public FetchNotUpdatedUserPropertiesHook(TContext context)
	{
		_context = context;
	}

	/// <inheritdoc />
	public async Task Handle(SienarUser user, ActionType action)
	{
		if (action is not ActionType.Update) return;

		var existingUser = await _context
			.Set<SienarUser>()
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Id == user.Id);
		if (existingUser is null) return;

		user.PasswordHash = existingUser.PasswordHash;
		user.EmailConfirmed = existingUser.EmailConfirmed;
		user.PendingEmail = existingUser.PendingEmail;
		user.LockoutEnd = existingUser.LockoutEnd;
		user.LoginFailedCount = existingUser.LoginFailedCount;
	}
}