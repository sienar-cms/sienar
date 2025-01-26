using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sienar.Extensions;
using Sienar.Identity;
using Sienar.Identity.Data;
using Sienar.Identity.Hooks;
using Sienar.Identity.Processors;
using Sienar.Media;
using Sienar.Media.Processors;

namespace Sienar.Plugins;

public class SienarCmsEfRepositoriesPlugin<TContext> : IPlugin
	where TContext : DbContext
{
	private readonly WebApplicationBuilder _builder;
	private readonly IPluginDataProvider _pluginDataProvider;

	public SienarCmsEfRepositoriesPlugin(
		WebApplicationBuilder builder,
		IPluginDataProvider pluginDataProvider)
	{
		_builder = builder;
		_pluginDataProvider = pluginDataProvider;
	}

	public void Configure()
	{
		_pluginDataProvider.Add(new PluginData
		{
			Name = "Sienar CMS Entity Framework Repositories",
			Version = Version.Parse("0.1.0"),
			Author = "Christian LeVesque",
			AuthorUrl = "https://levesque.dev",
			Description = "This plugin provides repositories to persist app data to any database provider supported by Entity Framework",
			Homepage = "https://sienar.io"
		});

		var services = _builder.Services;

		services.TryAddScoped<IVerificationCodeManager, VerificationCodeManager<TContext>>();
		services.TryAddScoped<IUserRepository, UserRepository<TContext>>();

		services
			.AddEntityFrameworkEntity<SienarUser, SienarUserFilterProcessor, IUserRepository, UserRepository<TContext>>()
			.AddBeforeActionHook<SienarUser, FetchNotUpdatedUserPropertiesHook<TContext>>()
			.AddEntityFrameworkEntityWithDefaultRepository<SienarRole, SienarRoleFilterProcessor, TContext>()
			.AddEntityFrameworkEntity<LockoutReason, LockoutReasonFilterProcessor, ILockoutReasonRepository, LockoutReasonRepository<TContext>>()
			.AddEntityFrameworkEntityWithDefaultRepository<Upload, UploadFilterProcessor, TContext>();
	}
}