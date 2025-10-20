using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Configuration;
using Sienar.Infrastructure;
using Sienar.Ui;

namespace Sienar.Plugins;

/// <summary>
/// Configures the Sienar app to run with pre-rendered Blazor WASM application support
/// </summary>
public class BlazorPlugin : IPlugin
{
	private readonly WebApplicationBuilder _builder;
	private readonly IConfigurer<RazorComponentsServiceOptions>? _blazorConfigurer;
	private readonly IEnumerable<IConfigurer<IRazorComponentsBuilder>> _additionalBlazorConfigurers;
	private readonly MiddlewareProvider _middlewareProvider;
	private readonly RoutableAssemblyProvider _routableAssemblyProvider;
	private readonly ComponentProvider _componentProvider;
	private readonly GlobalComponentProvider _globalComponentProvider;

	/// <summary>
	/// Creates a new instance of <c>BlazorPlugin</c>
	/// </summary>
	public BlazorPlugin(
		WebApplicationBuilder builder,
		IEnumerable<IConfigurer<IRazorComponentsBuilder>> additionalBlazorConfigurers,
		MiddlewareProvider middlewareProvider,
		RoutableAssemblyProvider routableAssemblyProvider,
		ComponentProvider componentProvider,
		GlobalComponentProvider globalComponentProvider,
		IConfigurer<RazorComponentsServiceOptions>? blazorConfigurer = null)
	{
		_builder = builder;
		_additionalBlazorConfigurers = additionalBlazorConfigurers;
		_middlewareProvider = middlewareProvider;
		_routableAssemblyProvider = routableAssemblyProvider;
		_componentProvider = componentProvider;
		_globalComponentProvider = globalComponentProvider;
		_blazorConfigurer = blazorConfigurer;
	}

	/// <inheritdoc />
	public void Configure()
	{
		_builder.Services
			.AddSingleton(_routableAssemblyProvider)
			.AddSingleton(_componentProvider)
			.AddSingleton(_globalComponentProvider)
			.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
		var blazorBuilder = _builder.Services
			.AddRazorComponents(o => _blazorConfigurer?.Configure(o))
			.AddInteractiveWebAssemblyComponents();

		foreach (var configurer in _additionalBlazorConfigurers)
		{
			configurer.Configure(blazorBuilder);
		}

		_middlewareProvider.AddWithPriority(
			Priority.Lowest,
			app =>
			{
				var blazorEndpointBuilder = app.MapRazorComponents<SienarApp>()
					.AddInteractiveWebAssemblyRenderMode();

				foreach (var assembly in _routableAssemblyProvider)
				{
					blazorEndpointBuilder.AddAdditionalAssemblies(assembly);
				}
			});
	}

	/// <summary>
	/// Adds necessary plugin and service dependencies to the Sienar app
	/// </summary>
	/// <param name="builder">The <see cref="SienarAppBuilder"/></param>
	[AppConfigurer]
	public static void ConfigureApp(SienarAppBuilder builder)
	{
		builder.AddPlugin<WebArchitecturePlugin>();
		builder.StartupServices
			.AddSingleton<RoutableAssemblyProvider>()
			.AddSingleton<GlobalComponentProvider>()
			.AddSingleton<ComponentProvider>();
	}
}
