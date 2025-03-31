using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Sienar.Identity;

public class VerificationCodeManager<TContext> : IVerificationCodeManager
	where TContext : DbContext
{
	private readonly TContext _context;
	private DbSet<SienarUser> UserSet => _context.Set<SienarUser>();

	public VerificationCodeManager(TContext context)
	{
		_context = context;
	}

	/// <inheritdoc/>
	public async Task<VerificationCode> CreateCode(
		SienarUser user,
		string type)
	{
		await UserSet
			.Entry(user)
			.Collection(u => u.VerificationCodes)
			.LoadAsync();

		await DeleteCode(user, type);

		var code = new VerificationCode
		{
			Code = Guid.NewGuid(),
			Type = type,
			ExpiresAt = GetVerificationCodeExpiration(type)
		};

		user.VerificationCodes.Add(code);
		UserSet.Update(user);
		await _context.SaveChangesAsync();

		return code;
	}

	/// <inheritdoc/>
	public Task DeleteCode(
		SienarUser user,
		string type)
		=> DeleteVerificationCode(user, type, true);

	private async Task DeleteVerificationCode(
		SienarUser user,
		string type,
		bool saveChanges)
	{
		await UserSet
			.Entry(user)
			.Collection(u => u.VerificationCodes)
			.LoadAsync();

		var code = user.VerificationCodes.FirstOrDefault(c => c.Type == type);
		if (code is null)
		{
			return;
		}

		user.VerificationCodes.Remove(code);
		_context.Remove(code);

		if (saveChanges)
		{
			await _context.SaveChangesAsync();
		}
	}

	/// <inheritdoc/>
	public async Task<VerificationCode?> GetCode(
		SienarUser user,
		string type)
	{
		await UserSet
			.Entry(user)
			.Collection(u => u.VerificationCodes)
			.LoadAsync();

		return user.VerificationCodes.FirstOrDefault(v => v.Type == type);
	}

	public VerificationCodeStatus GetCodeStatus(
		VerificationCode? code,
		Guid suppliedCode)
	{
		if (code is null 
		|| code.Code != suppliedCode)
		{
			return VerificationCodeStatus.Invalid;
		}

		if (code.ExpiresAt < DateTime.UtcNow)
		{
			return VerificationCodeStatus.Expired;
		}

		return VerificationCodeStatus.Valid;
	}

	public async Task<VerificationCodeStatus> GetCodeStatus(
		SienarUser user,
		string type,
		Guid suppliedCode)
	{
		var code = await GetCode(user, type);
		return GetCodeStatus(code, suppliedCode);
	}

	public async Task<VerificationCodeStatus> VerifyCode(
		SienarUser user,
		string type,
		Guid suppliedCode,
		bool deleteIfValid)
	{
		var code = await GetCode(user, type);
		var status = GetCodeStatus(code, suppliedCode);
		if (status == VerificationCodeStatus.Valid && deleteIfValid)
		{
			await DeleteCode(user, type);
		}

		return status;
	}

	private static DateTime GetVerificationCodeExpiration(string type)
	{
		return DateTime.UtcNow.AddMinutes(30);
	}
}