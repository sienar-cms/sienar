namespace Sienar.Infrastructure.Plugins;

public class SienarCmsRequestConfigurer : IRequestConfigurer
{
	private readonly IStyleProvider _styleProvider;
	private readonly IScriptProvider _scriptProvider;

	public SienarCmsRequestConfigurer(
		IStyleProvider styleProvider,
		IScriptProvider scriptProvider)
	{
		_styleProvider = styleProvider;
		_scriptProvider = scriptProvider;
	}

	/// <inheritdoc />
	public void Configure()
	{
		SetupStyles();
		SetupScripts();
	}

	private void SetupStyles()
		=> _styleProvider
			.Add("/_content/MudBlazor/MudBlazor.min.css")
			.Add("/_content/Sienar.UI/sienar.css")
			.Add("/_content/Sienar.UI/Sienar.UI.bundle.scp.css");

	private void SetupScripts()
		=> _scriptProvider
			.Add("/_content/MudBlazor/MudBlazor.min.js")
			.Add("/_content/Sienar.Plugin.Cms/sienar-cms.js");
}