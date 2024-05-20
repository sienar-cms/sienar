#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Errors;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class DeleteAccountProcessor : IProcessor<DeleteAccountRequest, bool>
{
	private readonly DbContext _context;
	private readonly IUserAccessor _userAccessor;
	private readonly IUserManager _userManager;

	public DeleteAccountProcessor(
		DbContext context,
		IUserAccessor userAccessor,
		IUserManager userManager)
	{
		_context = context;
		_userAccessor = userAccessor;
		_userManager = userManager;
	}

	public async Task<OperationResult<bool>> Process(DeleteAccountRequest request)
	{
		var userId = await _userAccessor.GetUserId();
		if (!userId.HasValue)
		{
			return this.Unauthorized(message: CmsErrors.Account.LoginRequired);
		}

		var user = await _userManager.GetSienarUser(userId.Value);
		if (user is null)
		{
			return this.Unauthorized(message: CmsErrors.Account.LoginRequired);
		}

		if (!await _userManager.VerifyPassword(user, request.Password))
		{
			return this.Unauthorized(message: CmsErrors.Account.LoginFailedInvalid);
		}

		_context
			.Set<SienarUser>()
			.Remove(user);
		await _context.SaveChangesAsync();
		return this.Success(true, "Account deleted successfully");
	}
}