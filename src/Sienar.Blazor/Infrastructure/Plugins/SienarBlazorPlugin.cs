using System;
using Sienar.UI;
using Sienar.UI.Views;

namespace Sienar.Infrastructure.Plugins;

public class SienarBlazorPlugin : ISienarPlugin
{
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
	public void SetupStyles(IStyleProvider styleProvider)
		=> styleProvider
			.Add("/_content/MudBlazor/MudBlazor.min.css")
			.Add("/_content/Sienar.Blazor/sienar.css")
			.Add("/_content/Sienar.BlazorUtils/Sienar.BlazorUtils.bundle.scp.css");

	/// <inheritdoc />
	public void SetupScripts(IScriptProvider scriptProvider)
		=> scriptProvider
			.Add("/_framework/blazor.server.js")
			.Add("/_content/MudBlazor/MudBlazor.min.js")
			.Add("/_content/Sienar.Blazor/sienar.js");

	/// <inheritdoc />
	public void SetupComponents(IComponentProvider componentProvider)
	{
		componentProvider.AppComponent = typeof(SienarBlazorServerApp);
		componentProvider.TopLevelComponents.Add(typeof(AuthStateRefresher));
		componentProvider.AuthorizingView = typeof(Authorizing);
		componentProvider.NotAuthorizedView = typeof(UnauthorizedRedirect);
	}
}