using Microsoft.AspNetCore.Components;

namespace Sienar.Extensions;

public static class NavigationManagerExtensions
{
	public static void ForceReload(
		this NavigationManager self,
		string destination)
	{
		self.NavigateTo($"/reload-hack?Destination={destination}");
	}
}