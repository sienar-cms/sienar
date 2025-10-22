using System;
using MailKit.Net.Smtp;
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
	private readonly IApplicationAdapter _adapter;
	private readonly IConfiguration _configuration;
	private readonly PluginDataProvider _pluginDataProvider;

	/// <summary>
	/// Creates a new instance of <c>MailKitPlugin</c>
	/// </summary>
	public MailKitPlugin(
		IApplicationAdapter adapter,
		IConfiguration configuration,
		PluginDataProvider pluginDataProvider)
	{
		_adapter = adapter;
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

		_adapter.AddServices(sp =>
		{
			sp
				.AddScoped<IEmailSender, MailKitSender>()
				.AddScoped<ISmtpClient, SmtpClient>()
				.ApplyDefaultConfiguration<SmtpOptions>(
					_configuration.GetSection("Sienar:Email:Smtp"));
		});
	}
}