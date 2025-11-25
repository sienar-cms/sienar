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
public class ChangePasswordProcessor<TContext> : IStatusProcessor<ChangePasswordRequest>
	where TContext : DbContext
{
	private readonly TContext _context;
	private readonly IUserAccessor _userAccessor;
	private readonly IPasswordManager _passwordManager;

	public ChangePasswordProcessor(
		TContext context,
		IUserAccessor userAccessor,
		IPasswordManager passwordManager)
	{
		_context = context;
		_userAccessor = userAccessor;
		_passwordManager = passwordManager;
	}

	public async Task<OperationResult<bool>> Process(ChangePasswordRequest request)
	{
		var userId = await _userAccessor.GetUserId();
		if (!userId.HasValue)
		{
			return new(
				OperationStatus.Unauthorized,
				message: CoreErrors.Account.LoginRequired);
		}

		var user = await _context
			.Set<SienarUser>()
			.FindAsync(userId.Value);
		if (user is null)
		{
			return new(
				OperationStatus.Unauthorized,
				message: CoreErrors.Account.LoginRequired);
		}

		if (!await _passwordManager.VerifyPassword(user, request.CurrentPassword))
		{
			return new(
				OperationStatus.Unauthorized,
				message: CoreErrors.Account.LoginFailedInvalid);
		}

		await _passwordManager.UpdatePassword(user, request.NewPassword);

		return new(
			OperationStatus.Success,
			true,
			"Password changed successfully");
	}
}