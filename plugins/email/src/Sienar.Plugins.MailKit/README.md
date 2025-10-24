# Sienar.MailKit plugin

This plugin configures your Sienar application to send email via SMTP through the [MailKit](https://github.com/jstedfast/MailKit) library for .NET.

## Configuration

You need to configure the MailKit plugin with your SMTP information in order to send mail. You can do this in one of two ways: via `appsettings.json` or directly on the `IServiceCollection` like normal. If you apply your own configuration, Sienar will detect an existing configuration and skip adding its own. All configuration is stored on the [SmtpOptions](https://github.com/sienar-cms/sienar/blob/main/plugins/mailkit/src/Sienar.Plugins.MailKit/SmtpOptions.cs) class.

### `appsettings.json` configuration

If you want to configure your SMTP info via `appsettings.json`, you need to add the following section to your `appsettings.json` file, maintaining the nested structure shown here:

```json
{
    "Sienar": {
        "Email": {
            "Smtp": {
                "Host": "your-smtp-host.dev",
                "Port": 587, 
                "SecureSocketOptions": 3,
                "Username": "your-smtp-username",
                "Password": "your-smtp-password"
            }
        }
    }
}
```

Replace these values with the appropriate values for your SMTP host. We recommend you supply sensitive information using other means such as environment variables or Azure secrets.

**NOTE:** The `SecureSocketOptions` value is an `int` between 0 and 4, and it represents a member of the [MailKit.Security.SecureSocketOptions](https://github.com/jstedfast/MailKit/blob/master/MailKit/Security/SecureSocketOptions.cs) enum.

### Standard configuration

If you want to configure your SMTP info directly on `IServiceCollection` yourself, you must do so before adding the `MailKitPlugin` to the Sienar app because if `MailKitPlugin` doesn't detect an existing configuration, it will apply its own. You can access the `IServicecollection` directly from `Program.cs` by calling `SienarWebAppBuilder.SetupDependencies(WebApplicationBuilder)` and accessing the `Services` property:

```csharp
// Program.cs

using MailKit.Security;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Email;
using Sienar.Extensions;
using Sienar.Infrastructure;

await SienarWebAppBuilder
	.Create(args, typeof(Program).Assembly)
	.SetupDependencies(builder =>
	{
		// configure SmtpOptions with an IConfiguration...
		builder.Services.Configure<SmtpOptions>(
			builder.Configuration.GetSection("Your:Custom:Config"));

		// ...or use an Action<SmtpOptions>
		builder.Services.Configure<SmtpOptions>(o =>
		{
			o.Host = "your-smtp-host.dev";
			o.Port = 587;
			o.SecureSocketOptions = SecureSocketOptions.StartTls;
			o.Username = "your-smtp-username";
			o.Password = "your-smtp-password";
		});
	})
	.AddPlugin<MailKitPlugin>()
	.BuildBlazor()
	.RunAsync();
```

Replace these values with the appropriate values for your SMTP host. We recommend you supply sensitive information using other means such as environment variables or Azure secrets. You can also feel free to supply your configuration via a custom plugin if your app's custom functionality is wrapped in a plugin.