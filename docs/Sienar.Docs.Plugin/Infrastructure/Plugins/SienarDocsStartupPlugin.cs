using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Sienar.Extensions;
using Sienar.Infrastructure.Articles;
using Sienar.State;

namespace Sienar.Infrastructure.Plugins;

// The only reason this class is internal is to hide it from Intellisense, so developers don't get confused about which plugin should be used
internal class SienarDocsStartupPlugin : ISienarClientStartupPlugin
{
	/// <inheritdoc />
	public void SetupDependencies(WebAssemblyHostBuilder builder)
	{
		builder.Services
			.AddScoped(_ =>
				new HttpClient
				{
					BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
				})
			.AddScoped<IUserAccessor, NullUserAccessor>()
			.AddScoped<AuthenticationStateProvider, DefaultAuthStateProvider>()
			.AddScoped<IArticleSeriesProvider, ArticleSeriesProvider>()
			.AddScoped<ArticleSeriesStateProvider>()
			.AddAuthorizationCore()
			.AddCascadingAuthenticationState()
			.AddMudServices();

		builder.RootComponents.Add<SienarApp>("#app");
		builder.RootComponents.Add<HeadOutlet>("head::after");
	}

	/// <inheritdoc />
	public void SetupApp(WebAssemblyHost app)
	{
		app.Services
			.GetRequiredService<IArticleSeriesProvider>()
			.AddSeries();
	}
}