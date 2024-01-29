using System.Threading.Tasks;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Identity.Hooks;

public class DeleteOwnAccountLogoutHook : IAfterDelete<SienarUser>
{
	private readonly IUserAccessor _userAccessor;
	private readonly ISignInManager _signInManager;

	public DeleteOwnAccountLogoutHook(IUserAccessor userAccessor, ISignInManager signInManager)
	{
		_userAccessor = userAccessor;
		_signInManager = signInManager;
	}

	/// <inheritdoc />
	public async Task<HookStatus> Handle(SienarUser entity)
	{
		if (entity.Id == _userAccessor.GetUserId())
		{
			await _signInManager.SignOut();
		}

		return HookStatus.Success;
	}
}