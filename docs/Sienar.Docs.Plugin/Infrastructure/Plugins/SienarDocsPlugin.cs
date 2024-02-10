using System;
using Sienar.Extensions;
using Sienar.Infrastructure.Menus;
using Sienar.Layouts;
using Sienar.UI.Views;

namespace Sienar.Infrastructure.Plugins;

public class SienarDocsPlugin : ISienarPlugin
{
	/// <inheritdoc />
	public static Type StartupPlugin => typeof(SienarDocsStartupPlugin);

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
	public void SetupMenu(IMenuProvider menuProvider)
	{
		menuProvider
			.AddDocsMenu();
	}

	/// <inheritdoc />
	public void SetupComponents(IComponentProvider componentProvider)
	{
		componentProvider.DefaultLayout = typeof(DocsLayout);
		componentProvider.AuthorizingView = typeof(EmptyView);
		componentProvider.NotAuthorizedView = typeof(EmptyView);
	}

	/// <inheritdoc />
	public void SetupRoutableAssemblies(IRoutableAssemblyProvider routableAssemblyProvider)
	{
		routableAssemblyProvider.Add(typeof(SienarDocsPlugin).Assembly);
	}
}