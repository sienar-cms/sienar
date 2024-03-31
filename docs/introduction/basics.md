# Basics

Sienar is designed to make creating pluggable apps easier. For that reason, the boilerplate for a Sienar app looks quite a bit different from a traditional ASP.NET app. The standard boilerplate looks about like this:

```csharp
// Program.cs

using Microsoft.Extensions.DependencyInjection;
using Sienar.Email;
using Sienar.Infrastructure.Plugins;
using Sienar.Infrastructure;

await SienarWebAppBuilder
	.Create(args, typeof(Program).Assembly)
	.AddRootDbContext<AppDbContext>(o => o.UseDb())
	.AddPlugin<SienarCmsPlugin>()
	.AddPlugin<MailKitPlugin>()
	.ConfigureTheme<CustomTheme>()
	.BuildBlazor()
	.RunAsync();
```

This boilerplate shows how to initialize a Sienar app, complete with login, account management, user moderation, SMTP email, and more. The core functionality for building a Sienar web application is defined by [SienarWebAppBuilder](xref:Sienar.Infrastructure.SienarWebAppBuilder) and its methods.

The `SienarCmsPlugin` adds the out-of-the-box functionality you would expect from a CMS. By adding this plugin, you get a full UI to manage your own user account, moderate other users, and other functionality like logging in, password reset, and more. If you want to use your own UI, don't use this plugin directly; instead, create your own plugin that uses the non-UI portions of this plugin. If you want to create your own login system and other functionality, don't use this plugin at all.

The `MailKitPlugin` adds SMTP mailing capabilities to your app. By default, Sienar doesn't know how to send emails to users, so you can either disable email entirely in `appsettings.json`, or you can enable mailing by using `MailKitPlugin`. You can also create your own mailing plugin using the guide at the end of this README, under `Customization > Creating your own mail sending plugin`.

## Overview of boilerplate
### `SienarWebAppBuilder.Create()`

This static method creates a new `SienarWebAppBuilder`. This uses a similar pattern to ASP.NET with the `WebApplication.CreateBuilder()` method (in fact, Sienar uses this internally). The `Create()` method expects two arguments: the `string[] args` argument to `Program.Main()`, and type `Assembly` containing your custom Blazor pages. In most apps, this will probably be the same assembly that initializes the Sienar app, so the boilerplate uses `typeof(Program).Assembly`. The second argument is nullable, so if you don't define custom Blazor pages - or if your custom pages are defined by a custom plugin - you can pass `null` instead.

### `SienarWebAppBuilder.AddRootDbContext()`

The `AddRootDbContext()` method has the same signature as [IServiceCollection.AddDbContext()](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.entityframeworkservicecollectionextensions.adddbcontext?view=efcore-8.0). This method is optional, but it does work that is required by Sienar - namely, Sienar expects to find a class of type `DbContext` in the DI container. Internally, Sienar uses `DbContext.Set<TEntity>()` to access entities, and it requests the `DbContext` that contains Sienar's entities via the base type `DbContext`. The `AddRootDbcontext()` method not only registers your main `DbContext` class (the one containing Sienar's entities), but it also adds it to the DI container as the type `DbContext` as well. If you don't want to use this method, you'll have to register your context as a `DbContext`.

### `SienarWebAppBuilder.AddPlugin()`

This method tells Sienar that you want to use a plugin. Plugins must implement the [IWebPlugin](xref:Sienar.Infrastructure.Plugins.IWebPlugin) interface for web apps. For now, suffice it to say that plugins have the ability to register services in the DI container, configure the `IApplicationBuilder` with middleware, and hook directly into core Sienar functionality, like adding menu items to the account dashboard or overriding certain components in the UI.

### `SienarWebAppBuilder.ConfigureTheme()`

This extension method allows you to set a `MudTheme` for use by MudBlazor. This is an extension method that is only available to apps using `Sienar.Plugin.Cms`, which requires the usage of MudBlazor if you use the provided UI.

This extension method has two overloads. The first overload accepts a generic type argument representing your MudBlazor `MudTheme`, and an optional `bool isDarkMode` argument that lets MudBlazor know if your app should use dark mode. This first overload requires your theme to have a default constructor, so if it doesn't, you'll have to use the second overload. The second overload accepts two arguments: a `MudTheme` instance, and the `bool isDarkMode` argument from the first overload.

Internally, Sienar calls the second extension method from the first with a new instance of your `MudTheme`. If you don't want to supply a custom theme, you can just use `MudTheme` directly.

### `SienarWebAppBuilder.BuildBlazor()`

This extension method performs the final step of the Sienar building process. It performs internal tasks such as configuring plugins and middleware. The return result is a regular ASP.NET `WebApplication` that has been configured to use Blazor United, so if you want to do any final tasks outside the Sienar scope like adding final middleware, you can do that before calling `WebApplication.RunAsync()`

You can read more in the [SienarWebAppBuilder documentation](xref:Sienar.Infrastructure.SienarWebAppBuilder).