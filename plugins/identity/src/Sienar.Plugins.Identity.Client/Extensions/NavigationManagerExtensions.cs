using Microsoft.AspNetCore.Components;

namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="NavigationManager"/> extension methods used by the <c>Sienar.Plugins.Identity.Client</c> assembly
/// </summary>
public static class NavigationManagerExtensions
{
	/// <summary>
	/// Forces the page to reload pursuant to ASP.NET Core bug #51398
	/// </summary>
	/// <param name="self">the navigation manager</param>
	/// <param name="destination">the URL that should </param>
	public static void ForceReload(
		this NavigationManager self,
		string destination)
	{
		self.NavigateTo($"/reload-hack?Destination={destination}");
	}
}