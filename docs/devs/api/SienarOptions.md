---
pageTitle: SienarOptions
blurb: "Documentation for the SienarOptions class"
tags:
  - api
---

# `SienarOptions` class

The `SienarOptions` class is used to provide core Sienar configuration values. Its values can be set either by executing an overload of `IServiceCollection.Configure<SienarOptions>()`, or by supplying an object in `appsettings.json` at `Sienar:Core`.

## Instance properties

### EnableEmail

A `bool` property indicating whether email should be enabled for the application. Defaults to `false`.

**NOTE**: If you enable email, you will need to register an `IEmailSender` that can actually send email, or else your application will not work properly. The [MailKit plugin](/devs/plugins/mailkit) enables your application to send SMTP email via the MailKit library.

### RegistrationOpen

A `bool` property indicating whether your application should allow new user registrations. Defaults to `true`.

### SiteName

A `string` property indicating the display name of your website. Defaults to an empty string.

### SiteUrl

A `string` property indicating the web URL of your website. Sienar uses this property when sending email to direct users to your website. Defaults to an empty string.