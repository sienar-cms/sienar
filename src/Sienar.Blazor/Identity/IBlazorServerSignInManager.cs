using System;
using System.Threading.Tasks;

namespace Sienar.Identity;

public interface IBlazorServerSignInManager : ISignInManager
{
	/// <summary>
	/// Ends the current user session if the supplied GUID matches the current user's ID
	/// </summary>
	/// <param name="id">The ID of the user to sign out</param>
	Task ForceSignOutIfCurrentUser(Guid id);
}