using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Sienar.Hooks;

namespace Sienar.Components;

/// <summary>
/// A component that performs actions at application startup, after the Blazor UI has rendered
/// </summary>
public class SienarStartupActor : ComponentBase
{
	[Inject]
	private IEnumerable<IBeforeTask<SienarStartupActor>> StartupActions { get; set; } = null!;

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (!firstRender) return;

		foreach (var action in StartupActions)
		{
			await action.Handle();
		}
	}
}