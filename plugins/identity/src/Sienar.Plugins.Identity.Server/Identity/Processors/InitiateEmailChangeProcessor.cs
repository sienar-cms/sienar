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
using Sienar.Processors;
using Sienar.Security;

namespace Sienar.Identity.Processors;

/// <exclude />
public class InitiateEmailChangeProcessor<TContext> : IStatusProcessor<InitiateEmailChangeRequest>
	where TContext : DbContext
{
	private readonly TContext _context;
	private readonly IPasswordManager _passwordManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly IUserAccessor _userAccessor;
	private readonly SienarOptions _sienarOptions;
	private readonly LoginOptions _loginOptions;

	public InitiateEmailChangeProcessor(
		TContext context,
		IPasswordManager passwordManager,
		IAccountEmailManager emailManager,
		IUserAccessor userAccessor,
		IOptions<SienarOptions> sienarOptions,
		IOptions<LoginOptions> loginOptions)
	{
		_context = context;
		_passwordManager = passwordManager;
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

		if (!await _passwordManager.VerifyPassword(user, request.ConfirmPassword))
		{
			return new(
				OperationStatus.Unauthorized,
				message: CoreErrors.Account.LoginFailedInvalid);
		}

		var shouldSendConfirmationEmail = SienarUserExtensions.ShouldSendEmailConfirmationEmail(
			_loginOptions,
			_sienarOptions);

		if (shouldSendConfirmationEmail)
		{
			user.PendingEmail = request.Email;
			user.NormalizedPendingEmail = request.Email.ToNormalized();
		}
		else
		{
			user.Email = request.Email;
		}

		userSet.Update(user);
		await _context.SaveChangesAsync();

		if (shouldSendConfirmationEmail)
		{
			if (!await _emailManager.SendEmailChangeConfirmationEmail(user))
			{
				return new(
					OperationStatus.Unknown,
					true,
					CoreErrors.Email.FailedToSend);
			}
		}

		return new(
			OperationStatus.Success,
			true,
			"Email change requested");
	}
}