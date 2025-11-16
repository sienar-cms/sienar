using Microsoft.Extensions.DependencyInjection;
using Sienar.Hooks;
using Sienar.Ui;

namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="IServiceCollection"/> extension methods for the <c>Sienar.Utils.Blazor</c> assembly
/// </summary>
public static class SienarUtilsBlazorServiceCollectionExtensions
{
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
