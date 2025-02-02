using Sienar.Infrastructure;

namespace Sienar.Plugins;

/// <summary>
/// Configures the Sienar app to support MudBlazor
/// </summary>
public class BulmaUiPlugin : IPlugin
{
	private readonly IStyleProvider _styleProvider;

	/// <summary>
	/// Creates a new instance of <c>MudBlazorPlugin</c>
	/// </summary>
	public BulmaUiPlugin(IStyleProvider styleProvider)
	{
		_styleProvider = styleProvider;
	}

	/// <inheritdoc />
	public void Configure()
	{
		_styleProvider.Add("https://cdn.jsdelivr.net/npm/bulma@1.0.2/css/bulma.min.css");
		_styleProvider.Add("https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.7.2/css/all.min.css");
	}
}
