﻿#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Email;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Data;
using Sienar.Identity.Data;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ConfirmAccountProcessor<TContext> : IStatusProcessor<ConfirmAccountRequest>
	where TContext : DbContext
{
	private readonly TContext _context;
	private readonly IVerificationCodeManager _vcManager;
	private readonly IAccountEmailManager _emailManager;
	private readonly SienarOptions _options;

	public ConfirmAccountProcessor(
		TContext context,
		IVerificationCodeManager vcManager,
		IAccountEmailManager emailManager,
		IOptions<SienarOptions> options)
	{
		_context = context;
		_vcManager = vcManager;
		_emailManager = emailManager;
		_options = options.Value;
	}

	public async Task<OperationResult<bool>> Process(ConfirmAccountRequest request)
	{
		var user = await _context.FindAsync<SienarUser>(request.UserId);
		if (user is null)
		{
			return new(
				OperationStatus.NotFound,
				message: CmsErrors.Account.AccountErrorInvalidId);
		}

		var status = await _vcManager.VerifyCode(
			user,
			VerificationCodeTypes.Email,
			request.VerificationCode,
			true);

		if (status == VerificationCodeStatus.Invalid)
		{
			return new(
				OperationStatus.NotFound,
				message: CmsErrors.Account.VerificationCodeInvalid);
		}

		if (status == VerificationCodeStatus.Expired)
		{
			if (_options.EnableEmail)
			{
				var errorMessage = await _emailManager.SendWelcomeEmail(user)
					? CmsErrors.Account.VerificationCodeExpired
					: CmsErrors.Email.FailedToSend;

				return new(
					OperationStatus.Unprocessable,
					message: errorMessage);
			}

			return new(
				OperationStatus.Unprocessable,
				message: CmsErrors.Account.VerificationCodeExpiredEmailDisabled);
		}

		// Code was valid
		user.EmailConfirmed = true;
		_context.Update(user);
		await _context.SaveChangesAsync();

		return new(
			OperationStatus.Success,
			true,
			"Account confirmed successfully");
	}
}