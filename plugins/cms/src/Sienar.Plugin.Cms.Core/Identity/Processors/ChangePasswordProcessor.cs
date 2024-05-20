#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Errors;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ChangePasswordProcessor : IProcessor<ChangePasswordRequest, bool>
{
	private readonly IUserManager _userManager;
	private readonly IUserAccessor _userAccessor;

	public ChangePasswordProcessor(
		IUserManager userManager,
		IUserAccessor userAccessor)
	{
		_userManager = userManager;
		_userAccessor = userAccessor;
	}

	public async Task<OperationResult<bool>> Process(ChangePasswordRequest request)
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

		if (!await _userManager.VerifyPassword(user, request.CurrentPassword))
		{
			return this.Unauthorized(message: CmsErrors.Account.LoginFailedInvalid);
		}

		await _userManager.UpdatePassword(user, request.NewPassword);

		return this.Success(true, "Password changed successfully");
	}
}