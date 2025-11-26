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
	/// Adds the necessary services to use an entity via a REST API repository
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TDto">The type of the DTO</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddRestfulEntity<TDto>(this IServiceCollection self)
		where TDto : EntityBase
	{
		self.TryAddScoped<IEntityReader<TDto>, RestEntityReader<TDto>>();
		self.TryAddScoped<IEntityWriter<TDto>, RestEntityWriter<TDto>>();

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