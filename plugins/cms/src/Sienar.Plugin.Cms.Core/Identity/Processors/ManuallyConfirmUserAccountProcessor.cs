#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Errors;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ManuallyConfirmUserAccountProcessor
	: IProcessor<ManuallyConfirmUserAccountRequest, bool>
{
	private readonly DbContext _context;
	private readonly IUserManager _userManager;

	public ManuallyConfirmUserAccountProcessor(
		DbContext context,
		IUserManager userManager)
	{
		_context = context;
		_userManager = userManager;
	}

	public async Task<OperationResult<bool>> Process(ManuallyConfirmUserAccountRequest request)
	{
		var user = await _userManager.GetSienarUser(request.UserId);
		if (user is null)
		{
			return this.NotFound(message: CmsErrors.Account.NotFound);
		}

		if (user.EmailConfirmed)
		{
			return this.Unprocessable(message: $"{user.Username}'s account is already confirmed");
		}

		user.EmailConfirmed = true;

		_context
			.Set<SienarUser>()
			.Update(user);
		await _context.SaveChangesAsync();

		return this.Success(true, $"Confirmed {user.Username}'s account");
	}
}