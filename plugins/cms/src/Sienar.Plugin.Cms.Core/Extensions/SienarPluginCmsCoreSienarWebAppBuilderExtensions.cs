using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Infrastructure;

namespace Sienar.Extensions;

public static class SienarPluginCmsCoreSienarWebAppBuilderExtensions
{
	/// <summary>
	/// Registers a <see cref="TContext"/> using the provided options
	/// </summary>
	/// <param name="self">the web app builder</param>
	/// <param name="dbContextOptionsConfigurer">an action to figure the <see cref="DbContextOptionsBuilder"/></param>
	/// <param name="dbContextLifetime">the service lifetime of the <see cref="TContext"/></param>
	/// <param name="dbContextOptionsLifetime">the service lifetime of the <see cref="DbContextOptions{TContext}"/></param>
	/// <typeparam name="TContext">the type of the <see cref="DbContext"/></typeparam>
	/// <returns>the <see cref="SienarWebAppBuilder"/></returns>
	public static SienarWebAppBuilder AddRootDbContext<TContext>(
		this SienarWebAppBuilder self,
		Action<DbContextOptionsBuilder>? dbContextOptionsConfigurer = null,
		ServiceLifetime dbContextLifetime = ServiceLifetime.Scoped,
		ServiceLifetime dbContextOptionsLifetime = ServiceLifetime.Scoped)
		where TContext : DbContext
	{
		const string hasRootContextKey = "Sienar.Plugin.Cms.Core.HasRootContext";
		if (self.CustomItems.TryGetValue(hasRootContextKey, out var hasrootContext)
			&& (bool)hasrootContext)
		{
			throw new InvalidOperationException("You can only have one root DbContext");
		}

		self.CustomItems[hasRootContextKey] = true;

		self.Builder.Services.AddDbContext<TContext>(
			dbContextOptionsConfigurer,
			dbContextLifetime,
			dbContextOptionsLifetime);

		var baseContextDefinition = new ServiceDescriptor(
			typeof(DbContext),
			sp => sp.GetRequiredService<TContext>(),
			dbContextLifetime);

		self.Builder.Services.Add(baseContextDefinition);

		return self;
	}
}