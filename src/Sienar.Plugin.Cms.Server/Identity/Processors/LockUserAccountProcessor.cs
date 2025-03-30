#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Data;
using Sienar.Email;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class LockUserAccountProcessor<TContext> : IStatusProcessor<LockUserAccountRequest>
	where TContext : DbContext
{
	private readonly TContext _context;
	private readonly IAccountEmailManager _emailManager;

	public LockUserAccountProcessor(
		TContext context,
		IAccountEmailManager emailManager)
	{
		_context = context;
		_emailManager = emailManager;
	}

	public async Task<OperationResult<bool>> Process(LockUserAccountRequest request)
	{
		var user = await _context
			.Set<SienarUser>()
			.Where(u => u.Id == request.UserId)
			.Include(u => u.LockoutReasons)
			.FirstOrDefaultAsync();

		if (user is null)
		{
			return new(
				OperationStatus.NotFound,
				message: CmsErrors.Account.NotFound);
		}

		var reasons = await _context
			.Set<LockoutReason>()
			.Where(l => request.Reasons.Contains(l.Id))
			.ToListAsync();
		if (reasons.Count != request.Reasons.Count)
		{
			return new(
				OperationStatus.NotFound,
				message: CmsErrors.LockoutReason.NotFound);
		}

		user.LockoutReasons.AddRange(reasons);
		user.LockoutEnd = request.EndDate ?? DateTime.MaxValue;
		_context.Update(user);
		await _context.SaveChangesAsync();

		if (!await _emailManager.SendAccountLockedEmail(user))
		{
			return new(
				OperationStatus.Success,
				true,
				$"User {user.Username} was locked successfully, but the email notification failed to send.");
		}

		return new(
			OperationStatus.Success,
			true,
			$"Locked user {user.Username} successfully");
	}
}