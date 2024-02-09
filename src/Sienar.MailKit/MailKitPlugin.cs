using System;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Infrastructure.Menus;
using Sienar.Infrastructure.Plugins;

namespace Sienar.Email;

public class MailKitPlugin : ISienarServerPlugin
{
	/// <inheritdoc />
	public PluginData PluginData { get; } = new()
	{
		Name = "Sienar MailKit",
		Version = Version.Parse("0.1.0"),
		Author = "Christian LeVesque",
		AuthorUrl = "https://levesque.dev",
		Description = "Sienar MailKit provides access to mail delivery services over SMTP using the MailKit library for .NET.",
		Homepage = "https://sienar.siteveyor.com/plugins/mailkit"
	};

	/// <inheritdoc />
	public void SetupDependencies(WebApplicationBuilder builder)
	{
		var services = builder.Services;

		services.AddScoped<ISmtpClient>(delegate { return new SmtpClient(); });

		services.RemoveService<IEmailSender>();
		services.AddScoped<IEmailSender, MailKitSender>();

		var smtpConfigurer = services.GetAndRemoveService<Action<SmtpOptions>>();
		if (smtpConfigurer is null)
		{
			services.Configure<SmtpOptions>(
				builder.Configuration.GetSection("Sienar:Email:Smtp"));
		}
		else
		{
			services.Configure(smtpConfigurer);
		}
	}

	/// <inheritdoc />
	public void SetupApp(WebApplication app) {}

	public bool PluginShouldExecute(
		HttpContext context,
		IPluginExecutionTracker executionTracker) => false;

	/// <inheritdoc />
	public void SetupStyles(IStyleProvider styleProvider) {}

	/// <inheritdoc />
	public void SetupScripts(IScriptProvider scriptProvider) {}

	/// <inheritdoc />
	public void SetupMenu(IMenuProvider menuProvider) {}

	/// <inheritdoc />
	public void SetupDashboard(IMenuProvider dashboardProvider) {}

	/// <inheritdoc />
	public void SetupComponents(IComponentProvider componentProvider) {}

	/// <inheritdoc />
	public void SetupRoutableAssemblies(IRoutableAssemblyProvider routableAssemblyProvider) {}
}