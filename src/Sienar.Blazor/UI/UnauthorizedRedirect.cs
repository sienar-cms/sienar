using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Sienar.Extensions;

namespace Sienar.UI;

public class UnauthorizedRedirect : ComponentBase
{
	[CascadingParameter]
	private Task<AuthenticationState> AuthState { get; set; } = default!;

	[Inject]
	private NavigationManager NavManager { get; set; } = default!;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		// No need to check first render, as this component should only ever render the once
		if (!(await AuthState).IsAuthenticated())
		{
			var currentUri = NavManager.ToBaseRelativePath(NavManager.Uri);
			currentUri = HttpUtility.UrlEncode(currentUri);
			NavManager.NavigateTo($"/dashboard/account/login?returnUri=/{currentUri}", true);
		}
		else
		{
			NavManager.NavigateTo("/dashboard/account/forbidden");
		}
	}
}