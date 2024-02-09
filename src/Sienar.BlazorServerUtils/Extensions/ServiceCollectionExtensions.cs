using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Plugins;
using Sienar.Infrastructure.Services;

namespace Sienar.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddSienarBlazorServerUtilities(
		this IServiceCollection self)
	{
		self.TryAddScoped<IPluginProvider, PluginProvider>();
		self.TryAddScoped<IStyleProvider, StyleProvider>();
		self.TryAddScoped<IScriptProvider, ScriptProvider>();
		self.TryAddScoped<IPluginExecutionTracker, PluginExecutionTracker>();
		self.TryAddScoped(typeof(IDbContextAccessor<>), typeof(DbContextAccessor<>));
		self.TryAddTransient(typeof(IEntityReader<>), typeof(EntityReader<>));
		self.TryAddTransient(typeof(IEntityWriter<>), typeof(EntityWriter<>));
		self.TryAddTransient(typeof(IEntityDeleter<>), typeof(EntityDeleter<>));
		self.TryAddTransient(typeof(IService<>), typeof(Service<>));
		self.TryAddTransient(typeof(IResultService<>), typeof(ResultService<>));

		return self;
	}

	/// <summary>
	/// Checks if a <see cref="TOptions"/> has already been configured, and if not, adds the supplied default configuration
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <param name="config">the default configuration to apply if no existing configuration was found</param>
	/// <typeparam name="TOptions">the type of the options class to configure</typeparam>
	/// <returns></returns>
	public static IServiceCollection ApplyDefaultConfiguration<TOptions>(
		this IServiceCollection self,
		IConfiguration config)
		where TOptions : class
	{
		if (!self.Any(sd => sd.ServiceType == typeof(IConfigureOptions<TOptions>)))
		{
			self.Configure<TOptions>(config);
		}

		return self;
	}
}