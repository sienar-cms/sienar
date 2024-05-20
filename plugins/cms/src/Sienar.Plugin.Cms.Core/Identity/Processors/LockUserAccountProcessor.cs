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
public class LockUserAccountProcessor : IProcessor<LockUserAccountRequest, bool>
{
	private readonly DbContext _context;
	private readonly IUserManager _userManager;

	public LockUserAccountProcessor(
		DbContext context,
		IUserManager userManager)
	{
		_context = context;
		_userManager = userManager;	
	}

	public async Task<OperationResult<bool>> Process(LockUserAccountRequest request)
	{
		var user = await _userManager.GetSienarUser(
			request.UserId,
			u => u.LockoutReasons);

		if (user is null)
		{
			return this.NotFound(message: CmsErrors.Account.NotFound);
		}

		foreach (var id in request.Reasons)
		{
			var reason = await _context
				.Set<LockoutReason>()
				.FindAsync(id);
			if (reason is null)
			{
				return this.NotFound(message: CmsErrors.LockoutReason.NotFound);
			}

			user.LockoutReasons.Add(reason);
		}

		user.LockoutEnd = request.EndDate;

		_context
			.Set<SienarUser>()
			.Update(user);
		await _context.SaveChangesAsync();

		return this.Success(true, $"Locked user {user.Username} successfully");
	}
}