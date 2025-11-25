#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ManuallyConfirmUserAccountProcessor<TContext>
	: IStatusProcessor<ManuallyConfirmUserAccountRequest>
	where TContext : DbContext
{
	private readonly TContext _context;

	public ManuallyConfirmUserAccountProcessor(
		TContext context)
	{
		_context = context;
	}

	public async Task<OperationResult<bool>> Process(ManuallyConfirmUserAccountRequest request)
	{
		var userSet = _context.Set<SienarUser>();
		var user = await userSet.FindAsync(request.UserId);
		if (user is null)
		{
			return new(
				OperationStatus.NotFound,
				message: CoreErrors.Account.NotFound);
		}

		if (user.EmailConfirmed)
		{
			return new(
				OperationStatus.Unprocessable,
				message: $"{user.Username}'s account is already confirmed");
		}

		user.EmailConfirmed = true;

		userSet.Update(user);
		await _context.SaveChangesAsync();
		return new(
			OperationStatus.Success,
			true,
			$"Confirmed {user.Username}'s account");
	}
}