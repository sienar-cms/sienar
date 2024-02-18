---
pageTitle: Basics
blurb: "An overview of the Sienar boilerplate"
pageNumber: 2
tags:
  - introduction
---

# Basics

Sienar is designed to make creating pluggable apps easier. For that reason, the boilerplate for a Sienar app looks quite a bit different from a traditional ASP.NET app. The standard boilerplate looks about like this:

```csharp
// Program.cs

using Microsoft.Extensions.DependencyInjection;
using Project.App;
using Project.Data;
using Sienar.Email;
using Sienar.Infrastructure.Plugins;
using Sienar.Infrastructure;

await SienarServerAppBuilder
	.Create<CustomDbContext>(
		args,
		o => o.UseSienarDb(),
		ServiceLifetime.Transient)
	.AddPlugin<SienarBlazorPlugin>()
	.AddPlugin<SienarCmsPlugin>()
	.AddStartupPlugin<MailKitPlugin>()
	.ConfigureTheme<CustomTheme>()
	.Build()
	.RunAsync();
```

This boilerplate shows how to initialize a Sienar app, complete with login, account management, user moderation, SMTP email, and more. The core functionality for building a Sienar application is defined by [SienarServerAppBuilder](/devs/api/SienarServerAppBuilder) and its extension methods.

The `SienarBlazorPlugin` adds core Sienar functionality to the application. With it, you can use data readers and writers to manage Sienar entities from code without a UI or other out-of-the-box functionality like login. You can set up code to use Sienar-provided classes like `ISignInManager` to implement your own log in rules if you want. Use the `SienarBlazorPlugin` alone if you want to create your own UI.

The `SienarCmsPlugin` adds the out-of-the-box functionality you would expect from a CMS. By adding this plugin, you get a full UI to manage your own user account, moderate other users, and other functionality like logging in, password reset, and more.

The `MailKitPlugin` adds SMTP mailing capabilities to your app. By default, Sienar doesn't know how to send emails to users, so you can either disable email entirely in `appsettings.json`, or you can enable mailing by using `MailKitPlugin`. You can also create your own mailing plugin using the guide at the end of this README, under `Customization > Creating your own mail sending plugin`

## Overview of boilerplate
### `SienarWebAppBuilder.Create()`

This static method creates a new `SienarWebAppBuilder`. This uses a similar pattern to ASP.NET with the `WebApplication.CreateBuilder()` method (in fact, Sienar uses this internally). There are two overloads to the `Create()` method: one that accepts the `string[] args` argument to `Program.Main()`, and one that accepts additional arguments that configure a `DbContext`. The second `Create()` overload accepts the same arguments as `IServiceCollection.AddDbContext<TContext>()` after the `string[] args` argument (because, again, Sienar uses this internally to register your DbContext). Sienar itself avoids a lot of generic code by requesting the base type `DbContext` from the DI container, so one thing that `Create<TContext>>()` does is re-registers your `TContext` as a `DbContext` so it can be used directly by Sienar without needing to know the implementation type.

The example code creates an app with a `CustomDbContext` that has a transient lifetime. This is recommended for Sienar applications that use the Blazor UI because a scoped `DbContext` is too long a lifetime in Blazor Server. Microsoft has [several workarounds](https://learn.microsoft.com/en-us/aspnet/core/blazor/blazor-ef-core?view=aspnetcore-8.0) for the service lifetime problem, but I find these to be too shallow for Sienar's needs. Internally, Sienar uses a *scoped* service to access the current `DbContext`, but refreshes the `DbContext` on page navigations by using a base page class - so even though the `DbContext` itself is transient, it is accessed via a scoped service that keeps it around for the proper duration (one page visit = one `DbContext`). This system is similar to how Microsoft solved Blazor scope issues with `OwningComponentBase`, but that class requires direct usage of `IServiceProvider`, or else limits you to a single component-scoped service.

### `SienarWebAppBuilder.AddPlugin()`

This extension method tells Sienar that you want to use a plugin. Your plugin is registered as scoped in the DI container, and Sienar instantiates it on every request.

Plugins must implement the `ISienarPlugin` interface, which is discussed later. For now, suffice it to say that plugins have the ability to register services in the DI container, configure the `IApplicationBuilder` with middleware, and hook directly into core Sienar functionality, like adding menu items to the account dashboard or overriding certain components in the UI.

### `SienarWebAppBuilder.ConfigureTheme()`

This extension method allows you to override the default `MudTheme` used by MudBlazor - if you don't care about the theme, or you're just prototyping for now, you can skip this method call and the default `MudTheme` will be used. You can also pass an optional `bool isDarkMode` as the second argument, which tells MudBlazor that your app should use dark mode. Both of these values end up stored in a scoped state provider, `ThemeState`.

### `SienarWebAppBuilder.Build()`

This method performs the final step of the Sienar building process. It performs internal tasks such as setting up the MudBlazor theme and configuring plugins and middleware. The return result is a regular ASP.NET `WebApplication`, so if you want to do any final tasks outside the Sienar scope like adding final middleware, you can do that before calling `WebApplication.RunAsync()`

You can read more in the [SienarWebAppBuilder documentation](/devs/api/SienarServerAppBuilder).