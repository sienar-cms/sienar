#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Email;
using Sienar.Errors;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class PerformEmailChangeProcessor
	: IProcessor<PerformEmailChangeRequest, bool>
{
	private readonly DbContext _context;
	private readonly IUserManager _userManager;
	private readonly IUserAccessor _userAccessor;
	private readonly IVerificationCodeManager _vcManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly SienarOptions _sienarOptions;

	public PerformEmailChangeProcessor(
		DbContext context,
		IUserManager userManager,
		IUserAccessor userAccessor,
		IVerificationCodeManager vcManager,
		IAccountEmailManager emailManager,
		IOptions<SienarOptions> sienarOptions)
	{
		_context = context;
		_userManager = userManager;
		_userAccessor = userAccessor;
		_vcManager = vcManager;
		_emailManager = emailManager;
		_sienarOptions = sienarOptions.Value;
	}

	public async Task<OperationResult<bool>> Process(PerformEmailChangeRequest request)
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

		if (user.Id != request.UserId)
		{
			return this.Unprocessable(
				message: CmsErrors.Account.AccountErrorWrongId);
		}

		if (string.IsNullOrEmpty(user.PendingEmail))
		{
			return this.Unprocessable(message: CmsErrors.Account.NoPendingEmail);
		}

		var status = await _vcManager.VerifyCode(
			user,
			VerificationCodeTypes.ChangeEmail,
			request.VerificationCode,
			true);

		if (status == VerificationCodeStatus.Invalid)
		{
			return this.NotFound(message: CmsErrors.Account.VerificationCodeInvalid);
		}

		if (status == VerificationCodeStatus.Expired)
		{
			if (_sienarOptions.EnableEmail)
			{
				await _emailManager.SendEmailChangeConfirmationEmail(user);
				return this.Unprocessable(message: CmsErrors.Account.VerificationCodeExpired);
			}

			return this.Unprocessable(message: CmsErrors.Account.VerificationCodeExpiredEmailDisabled);
		}

		// Code was valid
		user.Email = user.PendingEmail;
		user.PendingEmail = null;

		_context
			.Set<SienarUser>()
			.Update(user);
		await _context.SaveChangesAsync();

		return this.Success(true, "Email changed successfully");
	}
}