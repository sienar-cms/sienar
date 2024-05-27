using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sienar;

namespace Sevita.Tools.Pages;

[Route("/")]
[AllowAnonymous]
public class Index : ComponentBase
{
	[Inject]
	private NavigationManager NavManager { get; set; } = default!;

	/// <inheritdoc />
	protected override void OnInitialized()
		=> NavManager.NavigateTo(DashboardUrls.Index);
}