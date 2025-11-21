using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Security;
using Sienar.Ui;

namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="IServiceCollection"/> extension methods for the <c>Sienar.Utils.Blazor</c> assembly
/// </summary>
public static class SienarUtilsBlazorServiceCollectionExtensions
{
	/// <summary>
	/// Adds Sienar Blazor utilities to the DI contaziner
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddSienarBlazorUtilities(this IServiceCollection self)
	{
		self.TryAddScoped<SienarAuthenticationStateProvider>();
		self.TryAddScoped<AuthenticationStateProvider>(
			sp => sp.GetRequiredService<SienarAuthenticationStateProvider>());
		self.TryAddScoped<IDelegateHandler, DefaultDelegateHandler>();

		return self;
	}

	/// <summary>
	/// Adds a task to run once the Blazor UI has rendered and is ready to execute JavaScript
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="T">The type of the startup task</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddStartupTask<T>(this IServiceCollection self)
		where T : class, IBeforeTask<SienarStartupActor>
		=> self.AddScoped<IBeforeTask<SienarStartupActor>, T>();
}
