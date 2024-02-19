using System;
using Sienar.UI;
using Sienar.UI.Views;

namespace Sienar.Infrastructure.Plugins;

public class SienarBlazorPlugin : ISienarPlugin
{
	private readonly IStyleProvider _styleProvider;
	private readonly IScriptProvider _scriptProvider;
	private readonly IComponentProvider _componentProvider;

	public SienarBlazorPlugin(IStyleProvider styleProvider, IScriptProvider scriptProvider,
		IComponentProvider componentProvider)
	{
		_styleProvider = styleProvider;
		_scriptProvider = scriptProvider;
		_componentProvider = componentProvider;
	}

	/// <inheritdoc />
	public static Type StartupPlugin => typeof(SienarBlazorStartupPlugin);

	/// <inheritdoc />
	public PluginData PluginData { get; } = new()
	{
		Name = "Sienar Blazor",
		Version = Version.Parse("0.1.0"),
		Author = "Christian LeVesque",
		AuthorUrl = "https://levesque.dev",
		Description = "Sienar Blazor provides all of the main services and configuration required to operate the Sienar CMS. Sienar cannot serve a Blazor app without this plugin.",
		Homepage = "https://sienar.siteveyor.com"
	};

	/// <inheritdoc />
	public void Execute()
	{
		SetupStyles();
		SetupScripts();
		SetupComponents();
	}

	private void SetupStyles()
		=> _styleProvider
			.Add("/_content/MudBlazor/MudBlazor.min.css")
			.Add("/_content/Sienar.Blazor/sienar.css")
			.Add("/_content/Sienar.BlazorUtils/Sienar.BlazorUtils.bundle.scp.css");

	private void SetupScripts()
		=> _scriptProvider
			.Add("/_framework/blazor.server.js")
			.Add("/_content/MudBlazor/MudBlazor.min.js")
			.Add("/_content/Sienar.Blazor/sienar.js");

	private void SetupComponents()
	{
		_componentProvider.App = typeof(SienarBlazorServerApp);
		_componentProvider.TopLevelComponents.Add(typeof(AuthStateRefresher));
		_componentProvider.AuthorizingView = typeof(Authorizing);
		_componentProvider.NotAuthorizedView = typeof(UnauthorizedRedirect);
	}
}