using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Data;
using Sienar.Infrastructure;

namespace Sienar.Extensions;

public static class SienarRestServiceCollectionExtensions
{
	/// <summary>
	/// Adds the necessary services to use entities stored behind a REST API
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddRestfulEntities(this IServiceCollection self)
	{
		self.TryAddScoped(typeof(ICrudEndpointGenerator<>), typeof(DefaultCrudEndpointGenerator<>));
		self.TryAddScoped(typeof(IEntityReader<>), typeof(RestEntityReader<>));
		self.TryAddScoped(typeof(IEntityWriter<>), typeof(RestEntityWriter<>));
		self.TryAddScoped(typeof(IEntityDeleter<>), typeof(RestEntityDeleter<>));

		return self;
	}

	/// <summary>
	/// Adds the default <see cref="IRestClient"/> implementation to the DI container
	/// </summary>
	/// <param name="self">The <see cref="IServiceCollection"/></param>
	/// <returns>The <see cref="IServiceCollection"/></returns>
	public static IServiceCollection AddCookieRestClient(this IServiceCollection self)
		=> self.AddRestClient<CookieRestClient>();

	/// <summary>
	/// Adds the specified <see cref="IRestClient"/> implementation to the DI container
	/// </summary>
	/// <param name="self">The <see cref="IServiceCollection"/></param>
	/// <typeparam name="TClient">The type of the client</typeparam>
	/// <returns>The <see cref="IServiceCollection"/></returns>
	public static IServiceCollection AddRestClient<TClient>(this IServiceCollection self)
		where TClient : class, IRestClient
	{
		self.AddHttpClient<IRestClient, TClient>((sp, client) =>
		{
			var siteSettings = sp.GetRequiredService<IOptions<SienarOptions>>().Value;
			client.BaseAddress = new Uri($"{siteSettings.SiteUrl}/api/");
		});
		return self;
	}
}