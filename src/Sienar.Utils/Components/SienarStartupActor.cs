using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Sienar.Hooks;

namespace Sienar.Components;

/// <summary>
/// A component that performs actions at application startup, after the Blazor UI has rendered
/// </summary>
public class SienarStartupActor : ComponentBase
{
	[Inject]
	private IEnumerable<IBeforeTask<SienarStartupActor>> StartupActions { get; set; } = null!;

	[Inject]
	private ILogger<SienarStartupActor> Logger { get; set; } = null!;

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (!firstRender) return;

		Logger.LogInformation("SienarStartupActor callinga ctions");

		foreach (var action in StartupActions)
		{
			Logger.LogInformation("SienarStartupActorAction called");
			await action.Handle();
		}
	}
}