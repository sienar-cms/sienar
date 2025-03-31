#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Data;
using Sienar.Infrastructure;
using Sienar.Hooks;

namespace Sienar.Identity.Hooks;

/// <exclude />
public class EnsureAccountInfoUniqueValidator<TContext> : IStateValidator<SienarUser>,
	IStateValidator<RegisterRequest>
	where TContext : DbContext
{
	private readonly TContext _context;
	private readonly INotificationService _notifier;

	public EnsureAccountInfoUniqueValidator(
		TContext context,
		INotificationService notifier)
	{
		_context = context;
		_notifier = notifier;
	}

	Task<OperationStatus> IStateValidator<SienarUser>.Validate(SienarUser request, ActionType type)
		=> UserIsUnique(
			request.Username,
			request.Email,
			request.PendingEmail,
			request.Id);

	Task<OperationStatus> IStateValidator<RegisterRequest>.Validate(
		RegisterRequest request,
		ActionType action)
		=> UserIsUnique(
			request.Username,
			request.Email);

	private async Task<OperationStatus> UserIsUnique(
		string username,
		string email,
		string? pendingEmail = null,
		Guid id = default)
	{
		var normalizedUsername = username.ToUpperInvariant();
		var usernameIsTaken = await _context
			.Set<SienarUser>()
			.CountAsync(u => u.Id != id
				&& u.NormalizedUsername == normalizedUsername) > 0;
		if (usernameIsTaken)
		{
			_notifier.Error(CmsErrors.Account.UsernameTaken);
			return OperationStatus.Conflict;
		}

		var normalizedEmail = email.ToUpperInvariant();
		var normalizedPendingEmail = pendingEmail?.ToUpperInvariant();
		var emailIsTaken = await CheckIsEmailTaken(id, normalizedEmail) || (!string.IsNullOrEmpty(normalizedPendingEmail) && await CheckIsEmailTaken(id, normalizedPendingEmail));
		if (emailIsTaken)
		{
			_notifier.Error(CmsErrors.Account.EmailTaken);
			return OperationStatus.Conflict;
		}

		return OperationStatus.Success;
	}

	private async Task<bool> CheckIsEmailTaken(
		Guid id,
		string email)
	{
		return await _context
			.Set<SienarUser>()
			.CountAsync(
				u => u.Id != id
					&& (u.Email == email || u.PendingEmail == email)) > 0;
	}
}