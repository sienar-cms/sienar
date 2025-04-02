using System;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Extensions;
using Sienar.Infrastructure;
using Sienar.Plugins;

namespace Sienar.Email;

/// <summary>
/// Adds MailKit email support to Sienar applications
/// </summary>
public class MailKitPlugin : IPlugin
{
	private readonly WebApplicationBuilder _builder;
	private readonly IConfiguration _configuration;
	private readonly IPluginDataProvider _pluginDataProvider;

	/// <summary>
	/// Creates a new instance of <c>MailKitPlugin</c>
	/// </summary>
	public MailKitPlugin(
		WebApplicationBuilder builder,
		IConfiguration configuration,
		IPluginDataProvider pluginDataProvider)
	{
		_builder = builder;
		_configuration = configuration;
		_pluginDataProvider = pluginDataProvider;
	}

	/// <inheritdoc />
	public void Configure()
	{
		_pluginDataProvider.Add(new PluginData
		{
			Name = "Sienar MailKit",
			Version = Version.Parse("0.1.1"),
			Author = "Christian LeVesque",
			AuthorUrl = "https://levesque.dev",
			Description = "Sienar MailKit provides access to mail delivery services over SMTP using the MailKit library for .NET.",
			Homepage = "https://sienar.io/plugins/mailkit"
		});

		_builder.Services
			.AddScoped<IEmailSender, MailKitSender>()
			.AddScoped<ISmtpClient, SmtpClient>()
			.ApplyDefaultConfiguration<SmtpOptions>(
				_configuration.GetSection("Sienar:Email:Smtp"));
	}
}