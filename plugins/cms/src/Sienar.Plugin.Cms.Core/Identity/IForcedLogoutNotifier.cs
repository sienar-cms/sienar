using System;
using System.Threading.Tasks;

namespace Sienar.Identity;

public interface IForcedLogoutNotifier
{
	Task ForceLogoutUser(Guid userId);

	event Func<Guid, Task>? OnForceLogoutUser;
}