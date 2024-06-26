#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Email;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class RegisterProcessor : IProcessor<RegisterRequest, bool>
{
	private readonly DbContext _context;
	private readonly IAccountEmailManager _emailManager;
	private readonly IPasswordHasher<SienarUser> _passwordHasher;
	private readonly LoginOptions _loginOptions;
	private readonly SienarOptions _appOptions;

	public RegisterProcessor(
		DbContext context,
		IAccountEmailManager emailManager,
		IPasswordHasher<SienarUser> passwordHasher,
		IOptions<LoginOptions> loginOptions,
		IOptions<SienarOptions> appOptions)
	{
		_context = context;
		_emailManager = emailManager;
		_passwordHasher = passwordHasher;
		_loginOptions = loginOptions.Value;
		_appOptions = appOptions.Value;
	}

	public async Task<OperationResult<bool>> Process(RegisterRequest request)
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

		var shouldSendRegistrationEmail = SienarUserExtensions.ShouldSendEmailConfirmationEmail(
			_loginOptions,
			_appOptions);
		if (!shouldSendRegistrationEmail)
		{
			user.EmailConfirmed = true;
		}

		// Try to create that user with the given password
		await _context
			.Set<SienarUser>()
			.AddAsync(user);
		await _context.SaveChangesAsync();

		if (shouldSendRegistrationEmail) await _emailManager.SendWelcomeEmail(user);

		return this.Success(true, "Registered successfully");
	}
}