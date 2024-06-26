#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Email;
using Sienar.Errors;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ConfirmAccountProcessor : IProcessor<ConfirmAccountRequest, bool>
{
	private readonly DbContext _context;
	private readonly IUserManager _userManager;
	private readonly IVerificationCodeManager _vcManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly SienarOptions _options;

	public ConfirmAccountProcessor(
		DbContext context,
		IUserManager userManager,
		IVerificationCodeManager vcManager,
		IAccountEmailManager emailManager,
		IOptions<SienarOptions> options)
	{
		_context = context;
		_userManager = userManager;
		_vcManager = vcManager;
		_emailManager = emailManager;
		_options = options.Value;
	}

	public async Task<OperationResult<bool>> Process(ConfirmAccountRequest request)
	{
		var user = await _userManager.GetSienarUser(request.UserId);
		if (user is null)
		{
			return this.NotFound(message: CmsErrors.Account.AccountErrorInvalidId);
		}

		var status = await _vcManager.VerifyCode(
			user,
			VerificationCodeTypes.Email,
			request.VerificationCode,
			true);

		if (status == VerificationCodeStatus.Invalid)
		{
			return this.NotFound(message: CmsErrors.Account.VerificationCodeInvalid);
		}

		if (status == VerificationCodeStatus.Expired)
		{
			if (_options.EnableEmail)
			{
				var errorMessage = await _emailManager.SendWelcomeEmail(user)
					? CmsErrors.Account.VerificationCodeExpired
					: CmsErrors.Email.FailedToSend;

				return this.Unprocessable(message: errorMessage);
			}

			return this.Unprocessable(message: CmsErrors.Account.VerificationCodeExpiredEmailDisabled);
		}

		// Code was valid
		user.EmailConfirmed = true;
		_context
			.Set<SienarUser>()
			.Update(user);
		await _context.SaveChangesAsync();

		return this.Success(true, "Account confirmed successfully");
	}
}