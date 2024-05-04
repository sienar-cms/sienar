using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Sienar.Infrastructure.Plugins;

public class SienarBlazorPlugin : IWebPlugin
{
	/// <inheritdoc />
	public PluginData PluginData { get; } = new()
	{
		Name = "Sienar Blazor Plugin",
		Description = "A plugin that enables Sienar applications to run as Blazor United apps",
		Author = "Christian LeVesque",
		AuthorUrl = "https://levesque.dev",
		Homepage = "https://sienar.levesque.dev",
		Version = Version.Parse("0.1.0")
	};

	/// <inheritdoc />
	public void SetupDependencies(WebApplicationBuilder builder)
	{
		builder.Services
			.AddRazorComponents()
			.AddInteractiveServerComponents();
		builder.Services.AddCascadingAuthenticationState();
	}

	/// <inheritdoc />
	public void SetupApp(WebApplication app)
	{
		var p = app.Services.GetRequiredService<IRoutableAssemblyProvider>();

		app.UseAntiforgery();
		app
			.MapRazorComponents<SienarApp>()
			.AddInteractiveServerRenderMode()
			.AddAdditionalAssemblies(p.ToArray());
	}
}