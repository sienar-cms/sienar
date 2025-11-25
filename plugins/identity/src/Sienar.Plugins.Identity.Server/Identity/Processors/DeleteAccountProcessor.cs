#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Processors;
using Sienar.Security;

namespace Sienar.Identity.Processors;

/// <exclude />
public class DeleteAccountProcessor<TContext> : IStatusProcessor<DeleteAccountRequest>
	where TContext : DbContext
{
	private readonly IUserAccessor _userAccessor;
	private readonly TContext _context;
	private readonly IPasswordManager _passwordManager;
	private readonly ISignInManager _signInManager;

	public DeleteAccountProcessor(
		IUserAccessor userAccessor,
		TContext context,
		IPasswordManager passwordManager,
		ISignInManager signInManager)
	{
		_userAccessor = userAccessor;
		_context = context;
		_passwordManager = passwordManager;
		_signInManager = signInManager;
	}

	public async Task<OperationResult<bool>> Process(DeleteAccountRequest request)
	{
		var userId = await _userAccessor.GetUserId();
		if (!userId.HasValue)
		{
			return new(
				OperationStatus.Unauthorized,
				message: CoreErrors.Account.LoginRequired);
		}

		var userSet = _context.Set<SienarUser>();
		var user = await userSet.FindAsync(userId.Value);
		if (user is null)
		{
			return new(
				OperationStatus.Unauthorized,
				message: CoreErrors.Account.LoginRequired);
		}

		if (!await _passwordManager.VerifyPassword(user, request.Password))
		{
			return new(
				OperationStatus.Unauthorized,
				message: CoreErrors.Account.LoginFailedInvalid);
		}

		userSet.Remove(user);
		await _context.SaveChangesAsync();
		await _signInManager.SignOut();

		return new(
			OperationStatus.Success,
			true,
			"Account deleted successfully");
	}
}