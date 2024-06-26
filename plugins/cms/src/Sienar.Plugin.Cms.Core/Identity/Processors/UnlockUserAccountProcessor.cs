#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Errors;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class UnlockUserAccountProcessor : IProcessor<UnlockUserAccountRequest, bool>
{
	private readonly DbContext _context;
	private readonly IUserManager _userManager;

	public UnlockUserAccountProcessor(
		DbContext context,
		IUserManager userManager)
	{
		_context = context;
		_userManager = userManager;
	}

	public async Task<OperationResult<bool>> Process(UnlockUserAccountRequest request)
	{
		var user = await _userManager.GetSienarUser(
			request.UserId,
			u => u.LockoutReasons);
		if (user is null)
		{
			return this.NotFound(message: CmsErrors.Account.NotFound);
		}

		user.LockoutEnd = null;
		user.LockoutReasons.Clear();

		_context
			.Set<SienarUser>()
			.Update(user);
		await _context.SaveChangesAsync();

		return this.Success(true, $"User {user.Username}'s account was unlocked successfully");
	}
}