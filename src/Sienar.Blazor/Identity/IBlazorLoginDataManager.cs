using System;
using System.Threading.Tasks;

namespace Sienar.Identity;

public interface IBlazorLoginDataManager
{
	/// <summary>
	/// Writes a <see cref="BlazorServerLoginData"/> to the browser local store
	/// </summary>
	ValueTask WriteLoginData(BlazorServerLoginData loginData);

	/// <summary>
	/// Removes any <see cref="BlazorServerLoginData"/> from the browser local store
	/// </summary>
	ValueTask ClearLoginData();

	/// <summary>
	/// Creates a new <see cref="BlazorServerLoginData"/> using the supplied data
	/// </summary>
	/// <param name="userId">The ID of the user to create data for</param>
	/// <param name="isPersistent">Whether the login should be persistent</param>
	/// <param name="isAuthenticated">Whether the user is authenticated</param>
	/// <returns>the <see cref="BlazorServerLoginData"/></returns>
	BlazorServerLoginData CreateLoginData(
		Guid userId,
		bool isPersistent = false,
		bool isAuthenticated = false);

	/// <summary>
	/// Loads the current user's login status into the <see cref="AuthStateProvider"/>
	/// </summary>
	/// <returns>the user account if the user is logged in, else <c>null</c></returns>
	Task<SienarUser?> LoadUserLoginStatus();

	/// <summary>
	/// Refreshes the current user's login status as stored in the browser storage
	/// </summary>
	ValueTask RefreshUserLoginStatus();
}