using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
}