using System;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Extensions;
using Sienar.Infrastructure.Plugins;

namespace Sienar.Email;

public class MailKitPlugin : IWebPlugin
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

		services.RemoveService<IEmailSender>();
		services.AddScoped<IEmailSender, MailKitSender>();
		services.AddScoped<ISmtpClient, SmtpClient>();

		services.ApplyDefaultConfiguration<SmtpOptions>(
			builder.Configuration.GetSection("Sienar:Email:Smtp"));
	}
}