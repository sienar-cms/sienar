using Microsoft.AspNetCore.Builder;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Plugins;

namespace Sienar.Extensions;

public static class ApplicationBuilderExtensions
{
	public static void UsePluginMiddleware<TPlugin>(this IApplicationBuilder self)
		where TPlugin : ISienarPlugin
		=> self.UseMiddleware<SienarPluginMiddleware<TPlugin>>();
}