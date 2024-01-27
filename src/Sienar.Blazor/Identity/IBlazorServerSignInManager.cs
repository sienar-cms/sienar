using System.Threading.Tasks;

namespace Sienar.Identity;

public interface IBlazorServerSignInManager : ISignInManager
{
	/// <summary>
	/// Loads the current user's login status into the <see cref="AuthStateProvider"/>
	/// </summary>
	Task LoadUserLoginStatus();

	/// <summary>
	/// Refreshes the current user's login status as stored in the browser storage
	/// </summary>
	Task RefreshUserLoginStatus();
}