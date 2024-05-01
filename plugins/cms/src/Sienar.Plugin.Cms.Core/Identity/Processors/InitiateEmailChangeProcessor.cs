#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Email;
using Sienar.Errors;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;
using Sienar.Infrastructure.Services;

namespace Sienar.Identity.Processors;

/// <exclude />
public class InitiateEmailChangeProcessor : DbService<SienarUser>,
	IProcessor<InitiateEmailChangeRequest, bool>
{
	private readonly IUserManager _userManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly IUserAccessor _userAccessor;
	private readonly SienarOptions _sienarOptions;
	private readonly LoginOptions _loginOptions;

	public InitiateEmailChangeProcessor(
		DbContext context,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IUserManager userManager,
		IAccountEmailManager emailManager,
		IUserAccessor userAccessor,
		IOptions<SienarOptions> sienarOptions,
		IOptions<LoginOptions> loginOptions) : base(context, logger, notifier)
	{
		_userManager = userManager;
		_emailManager = emailManager;
		_userAccessor = userAccessor;
		_sienarOptions = sienarOptions.Value;
		_loginOptions = loginOptions.Value;
	}

	public async Task<OperationResult<bool>> Process(InitiateEmailChangeRequest request)
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

		if (!await _userManager.VerifyPassword(user, request.ConfirmPassword))
		{
			return this.Unauthorized(message: CmsErrors.Account.LoginFailedInvalid);
		}

		var shouldSendConfirmationEmail = SienarUserExtensions.ShouldSendEmailConfirmationEmail(
			_loginOptions,
			_sienarOptions);

		if (shouldSendConfirmationEmail)
		{
			user.PendingEmail = request.Email;
		}
		else
		{
			user.Email = request.Email;
		}

		EntitySet.Update(user);
		await Context.SaveChangesAsync();

		if (shouldSendConfirmationEmail)
		{
			if (!await _emailManager.SendEmailChangeConfirmationEmail(user))
			{
				return this.Unknown(true, CmsErrors.Email.FailedToSend);
			}
		}

		return this.Success(true, "Email change requested");
	}
}