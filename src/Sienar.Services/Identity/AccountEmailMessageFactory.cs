using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sienar.Email;

namespace Sienar.Identity;

public class AccountEmailMessageFactory : IAccountEmailMessageFactory
{
	protected readonly EmailOptions Options;
	protected readonly IAccountUrlProvider UrlProvider;

	public AccountEmailMessageFactory(
		IOptions<EmailOptions> options,
		IAccountUrlProvider urlProvider)
	{
		Options = options.Value;
		UrlProvider = urlProvider;
	}

	/// <inheritdoc/>
	public Task<string> WelcomeEmailHtml(
		string username,
		Guid userId,
		Guid code)
	{
		var message = $"<!DOCTYPE html><html><head><title>Welcome,{username}!</title></head><body><p>Hello {username},</p><p>Thank you for registering. Before you can sign in, you need to confirm your account by clicking the following link: <a href='{GenerateEmailConfirmationLink(userId, code)}'>confirm account</a></p><p>Regards,</p><p>{Options.Signature}</p></body></html>";

		return Task.FromResult(message);
	}

	/// <inheritdoc/>
	public Task<string> WelcomeEmailText(
		string username,
		Guid userId,
		Guid code)
	{
		var message = $"Hello {username},\n\n" +
		       $"Thank you for registering. Before you can sign in, you need to confirm your account by copying the following link into your browser: {GenerateEmailConfirmationLink(userId, code)}\n\n" +
		       "Regards,\n\n" +
		       Options.Signature;

		return Task.FromResult(message);
	}

	private string GenerateEmailConfirmationLink(Guid userId, Guid code)
	{
		return $"{UrlProvider.ConfirmationUrl}?userId={userId}&code={code}";
	}

	/// <inheritdoc/>
	public Task<string> ChangeEmailHtml(string username, Guid userId, Guid code)
	{
		var message = $"<!DOCTYPE html><html><head><title>Confirm your new email address, {username}!</title></head><body><p>Hello {username},</p><p>Before you can sign in using your new email address, you need to confirm it by clicking the following link: <a href='{GenerateChangeEmailConfirmationLink(userId, code)}'>confirm account</a></p><p>Regards,</p><p>{Options.Signature}</p></body></html>";

		return Task.FromResult(message);
	}

	/// <inheritdoc/>
	public Task<string> ChangeEmailText(string username, Guid userId, Guid code)
	{
		var message = $"Hello {username},\n\n" +
		       $"Before you can sign in using your new email address, you need to confirm it by copying the following link into your browser: {GenerateChangeEmailConfirmationLink(userId, code)}\n\n" +
		       "Regards,\n\n" +
		       Options.Signature;

		return Task.FromResult(message);
	}

	private string GenerateChangeEmailConfirmationLink(Guid userId, Guid code)
	{
		return $"{UrlProvider.EmailChangeUrl}?userId={userId}&code={code}";
	}

	/// <inheritdoc/>
	public Task<string> ResetPasswordHtml(string username, Guid userId, Guid code)
	{
		var message = $"<!DOCTYPE html><html><head><title>Password Reset Request</title></head><body><p>Hello {username},</p><p>We received a request to reset your password. If this was you, you can reset your password by clicking the following link: <a href='{GenerateResetPasswordLink(userId, code)}'>reset password</a></p><p>If this was not you, delete this email. The reset code will expire in 30 minutes and your account details will not be changed.</p><p>Regards,</p><p>{Options.Signature}</p></body></html>";

		return Task.FromResult(message);
	}

	/// <inheritdoc/>
	public Task<string> ResetPasswordText(string username, Guid userId, Guid code)
	{
		var message = $"Hello {username},\n\n" +
		       $"We received a request to reset your password. If this was you, you can reset your password by copying the following link into your browser: {GenerateResetPasswordLink(userId, code)}\n\n" +
		       "If this was not you, delete this email. The reset code will expire in 30 minutes and your account details will not be changed.\n\n" +
		       "Regards,\n\n" +
		       Options.Signature;

		return Task.FromResult(message);
	}

	private string GenerateResetPasswordLink(Guid userId, Guid code)
	{
		return $"{UrlProvider.ResetPasswordUrl}?userId={userId}&code={code}";
	}
}