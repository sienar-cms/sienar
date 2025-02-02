using Sienar.Infrastructure;

namespace Sienar.Plugins;

/// <summary>
/// Configures the Sienar app to support MudBlazor
/// </summary>
public class MudBlazorPlugin : IPlugin
{
	private readonly IApplicationAdapter _adapter;
	private readonly IScriptProvider _scriptProvider;
	private readonly IStyleProvider _styleProvider;

	/// <summary>
	/// Creates a new instance of <c>MudBlazorPlugin</c>
	/// </summary>
	public MudBlazorPlugin(
		IApplicationAdapter adapter,
		IScriptProvider scriptProvider,
		IStyleProvider styleProvider)
	{
		_adapter = adapter;
		_scriptProvider = scriptProvider;
		_styleProvider = styleProvider;
	}

	/// <inheritdoc />
	public void Configure()
	{
		_scriptProvider.Add(new ScriptResource
		{
			Src = "/_content/MudBlazor/MudBlazor.min.js",
			ShouldDefer = true
		});

		_styleProvider.Add("/_content/MudBlazor/MudBlazor.min.css");
	}
}
