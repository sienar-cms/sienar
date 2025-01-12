using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sienar.Components;
using Sienar.Hooks;
using Sienar.Infrastructure;

namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="IServiceCollection"/> extension methods for the <c>Sienar.Utils.Blazor</c> assembly
/// </summary>
public static class SienarUtilsBlazorServiceCollectionExtensions
{
	/// <summary>
	/// Adds Blazor-specific services to the service collection
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddSienarBlazorUtilities(this IServiceCollection self)
	{
		self.TryAddScoped<AuthStateProvider>();
		self.TryAddScoped<AuthenticationStateProvider>(
			sp => sp.GetRequiredService<AuthStateProvider>());
		self.TryAddScoped<IUserAccessor, BlazorUserAccessor>();

		return self
			.AddCascadingAuthenticationState()
			.AddAuthorizationCore();
	}

	/// <summary>
	/// Adds a task to run once the Blazor UI has rendered and is ready to execute JavaScript
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="T">The type of the startup task</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddStartupTask<T>(this IServiceCollection self)
		where T : class, IBeforeTask<SienarStartupActor>
		=> self.AddScoped<T>();
}
