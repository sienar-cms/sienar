using System;
using System.Threading.Tasks;

namespace Sienar.Identity;

public class BlazorForcedLogoutNotifier : IForcedLogoutNotifier
{
	/// <inheritdoc />
	public async Task ForceLogoutUser(Guid userId)
	{
		if (OnForceLogoutUser != null) await OnForceLogoutUser.Invoke(userId);
	}

	/// <inheritdoc />
	public event Func<Guid, Task>? OnForceLogoutUser;
}