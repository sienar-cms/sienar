# Sienar.MailKit plugin

This plugin configures your Sienar application to send email via SMTP through the [MailKit](https://github.com/jstedfast/MailKit) library for .NET.

## Configuration

You need to configure the MailKit plugin with your SMTP information in order to send mail. You can do this in one of two ways: via `appsettings.json` and via an `Action<SmtpOptions>`. All configuration is stored on the [SmtpOptions class](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.MailKit/SmtpOptions.cs).

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

### `Action<SmtpOptions>` configuration

If you want to configure your SMTP info via an `Action<SmtpOptions>`, you need to register an `Action<SmtpOptions>` as a singleton in the DI container before adding the `MailKitPlugin` to the Sienar app.

```csharp
await SienarServerAppBuilder
    .Create(args)
    // Other Sienar calls and plugins
    .AddServices(sp =>
    {
        sp.AddSingleton<Action<SmtpOptions>>(o =>
        {
           o.Host = "your-smtp-host.dev";
           o.Port = 587;
           o.SecureSocketOptions = SecureSocketOptions.StartTls;
           o.Username = "your-smtp-username";
           o.Password = "your-smtp-password"; 
        });
    })
    .AddPlugin(new MailKitPlugin())
    .Build()
    .RunAsync();
```

Replace these values with the appropriate values for your SMTP host. We recommend you supply sensitive information using other means such as environment variables or Azure secrets. You can also feel free to supply your configuration via a custom plugin if your app's custom functionality is wrapped in a plugin.