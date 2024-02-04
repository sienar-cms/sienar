using Microsoft.Extensions.DependencyInjection;
using Sienar.Infrastructure.Plugins;

namespace Sienar.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddBlazorWasmUtils(this IServiceCollection self)
	{
		self
			.AddSienarBlazorUtilities()
			.AddSingleton<IRootComponentProvider, RootComponentProvider>();

		return self;
	}
}