using Microsoft.AspNetCore.Components.Authorization;

namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="AuthenticationState"/> extension methods used by the <c>Sienar.Plugins.Identity.Client</c> assembly
/// </summary>
public static class AuthenticationStateExtensions
{
	/// <summary>
	/// Determines whether the current user is authenticated based on the <see cref="AuthenticationState"/>
	/// </summary>
	/// <param name="authState">the cascaded authentication state</param>
	/// <returns><c>true</c> if the user is authenticated, else <c>false</c></returns>
	public static bool IsAuthenticated(this AuthenticationState authState)
		=> authState.User.Identity?.IsAuthenticated ?? false;
}