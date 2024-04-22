using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MudBlazor.Services;
using Sienar.Infrastructure;

namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="IServiceCollection"/> extension methods for the <c>Sienar.Plugin.Cms.Blazor</c> assembly
/// </summary>
public static class SienarPluginCmsServiceCollectionExtensions
{
	/// <summary>
	/// Adds Blazor-specific services for Sienar CMS
	/// </summary>
	/// <param name="self">the <see cref="IServiceCollection"/></param>
	/// <returns>the <see cref="IServiceCollection"/></returns>
	public static IServiceCollection AddSienarCmsBlazor(this IServiceCollection self)
	{
		self.TryAddTransient<INotificationService, NotificationService>();

		var mudblazorConfigurer = self.GetAndRemoveService<Action<MudServicesConfiguration>>();

		return self.AddMudServices(mudblazorConfigurer ?? delegate {});
	}
}