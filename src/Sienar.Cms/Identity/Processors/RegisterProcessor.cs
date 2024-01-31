using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Email;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;
using Sienar.Infrastructure.Services;

namespace Sienar.Identity.Processors;

public class RegisterProcessor : DbService<SienarUser>, IProcessor<RegisterRequest>
{
	private readonly IVerificationCodeManager _verificationCodeManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly IPasswordHasher<SienarUser> _passwordHasher;
	private readonly LoginOptions _loginOptions;
	private readonly SienarOptions _appOptions;

	/// <inheritdoc />
	public RegisterProcessor(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier,
		IVerificationCodeManager verificationCodeManager,
		IAccountEmailManager emailManager,
		IPasswordHasher<SienarUser> passwordHasher,
		IOptions<LoginOptions> loginOptions,
		IOptions<SienarOptions> appOptions)
		: base(contextAccessor, logger, notifier)
	{
		_verificationCodeManager = verificationCodeManager;
		_emailManager = emailManager;
		_passwordHasher = passwordHasher;
		_loginOptions = loginOptions.Value;
		_appOptions = appOptions.Value;
	}

	public async Task<HookStatus> Process(RegisterRequest request)
	{
		// Checks passed. Make a new user
		var user = new SienarUser
		{
			Username = request.Username,
			Email = request.Email,
			ConcurrencyStamp = Guid.NewGuid()
		};

		user.PasswordHash = _passwordHasher.HashPassword(
			user,
			request.Password);

		var shouldSendRegistrationEmail = AccountUtils.ShouldSendEmailConfirmationEmail(
			_loginOptions,
			_appOptions);
		if (!shouldSendRegistrationEmail)
		{
			user.EmailConfirmed = true;
		}

		// Try to create that user with the given password
		await EntitySet.AddAsync(user);
		await Context.SaveChangesAsync();

		if (shouldSendRegistrationEmail)
		{
			await user.SendWelcomeEmail(
				_verificationCodeManager,
				_emailManager);
		}

		return HookStatus.Success;
	}

	/// <inheritdoc />
	public void NotifySuccess()
	{
		Notifier.Success("Registered successfully");
	}

	/// <inheritdoc />
	public void NotifyFailure()
	{
		Notifier.Error("An unknown error occurred while registering");
	}

	/// <inheritdoc />
	public void NotifyNoPermission()
	{
		Notifier.Error("You do not have permission to register");
	}
}