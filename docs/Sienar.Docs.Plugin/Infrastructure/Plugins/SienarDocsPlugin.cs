using System;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Sienar.Extensions;
using Sienar.Infrastructure.Articles;
using Sienar.Infrastructure.Menus;
using Sienar.Layouts;
using Sienar.State;
using Sienar.UI.Views;

namespace Sienar.Infrastructure.Plugins;

public class SienarDocsPlugin : ISienarClientPlugin
{
	/// <inheritdoc />
	public PluginData PluginData { get; } = new()
	{
		Author = "Christian LeVesque",
		AuthorUrl = "https://levesque.dev",
		Description = "The core Sienar docs plugin",
		Homepage = "https://sienar.levesque.dev",
		Name = "Sienar Documentation",
		Version = Version.Parse("0.1.0")
	};

	/// <inheritdoc />
	public PluginSettings PluginSettings { get; } = new()
	{
		HasRoutableComponents = true,
		UsesProviders = true
	};

	/// <inheritdoc />
	public void SetupStyles(IStyleProvider styleProvider) {}

	/// <inheritdoc />
	public void SetupScripts(IScriptProvider scriptProvider) {}

	/// <inheritdoc />
	public void SetupMenu(IMenuProvider menuProvider)
	{
		menuProvider
			.AddDocsMenu();
	}

	/// <inheritdoc />
	public void SetupDashboard(IMenuProvider dashboardProvider) {}

	/// <inheritdoc />
	public void SetupComponents(IComponentProvider componentProvider)
	{
		componentProvider.DefaultLayout = typeof(DocsLayout);
		componentProvider.AuthorizingView = typeof(EmptyView);
		componentProvider.NotAuthorizedView = typeof(EmptyView);
	}

	/// <inheritdoc />
	public void SetupDependencies(WebAssemblyHostBuilder builder)
	{
		builder.Services
			.AddScoped<IUserAccessor, NullUserAccessor>()
			.AddScoped<AuthenticationStateProvider, DefaultAuthStateProvider>()
			.AddScoped<IArticleSeriesProvider, ArticleSeriesProvider>()
			.AddScoped<ArticleSeriesStateProvider>()
			.AddAuthorizationCore()
			.AddCascadingAuthenticationState()
			.AddMudServices();
	}

	/// <inheritdoc />
	public void SetupApp(WebAssemblyHost app)
	{
		app.Services
			.GetRequiredService<IArticleSeriesProvider>()
			.AddIntroductionSeries();
	}

	/// <inheritdoc />
	public void SetupRootComponents(IRootComponentProvider rootComponentProvider)
	{
		rootComponentProvider
			.AddRootComponent("#app", typeof(SienarApp))
			.AddRootComponent("head::after", typeof(HeadOutlet));
	}
}