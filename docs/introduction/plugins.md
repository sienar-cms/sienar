# Plugins

Sienar offers plugin-like functionality via the [IWebPlugin](xref:Sienar.Infrastructure.Plugins.IWebPlugin) and [IDesktopPlugin](xref:Sienar.Infrastructure.Plugins.IDesktopPlugin) interfaces. These interfaces define methods and properties that Sienar uses to let your plugin tap into specific parts of your application startup in a distributable way.

As you might guess, the `IWebPlugin` is intended for web applications and the `IDesktopPlugin` is intended for desktop applications. Sienar web apps are built on top of ASP.NET Core, so `IWebPlugin` uses [WebApplicationBuilder](xref:Microsoft.AspNetCore.Builder.WebApplicationBuilder) and [WebApplication](xref:Microsoft.AspNetCore.Builder.WebApplication); Sienar desktop apps are built on top of .NET MAUI, so `IDesktopPlugin` uses [MauiAppBuilder](xref:Microsoft.Maui.Hosting.MauiAppBuilder) and [MauiApp](xref:Microsoft.Maui.Hosting.MauiApp).

Both interfaces define methods called `SetupDependencies()` and `SetupApp()`. The signatures for web plugins are `IWebPlugin.SetupDependencies(WebApplicationBuilder)` and `IWebPlugin.SetupApp(WebApplication)`. The signatures for desktop plugins are `IDesktopPlugin.SetupDependencies(MauiAppBuilder)` and `IDesktopPlugin.SetupApp(MauiApp)`.

The `SetupDependencies()` method is primarily used to configure the DI container, but since it receives the respective app builder, it can be used to do anything you would do with a `WebApplicationBuilder` or `MauiAppBuilder`.

Likewise, the `SetupApp()` method is primarily used to add middlewares and configure instances of singleton services in the DI container, but since it receive the respective app instance, it can be used to do anything you would do with a `WebApplication` or `MauiApp`.

Both interfaces also define a property `PluginData`, which is of type [PluginData](xref:Sienar.Infrastructure.Plugins.PluginData). This required property is used to identify a plugin in the [IPluginDataProvider](xref:Sienar.Infrastructure.Plugins.IPluginDataProvider), which is a singleton store for all loaded plugins.

## What's the point?

This plugin system is pretty bare. All it really does is exactly what ASP.NET already does. So what's the point?

### More easily consumed libraries

Really, all this plugin system is designed to do is to make it easier for application developers to consume distributed code. The current solution in .NET apps is extension methods on `IServiceCollection` and `IApplicationBuilder`, which is a fine solution. However, this requires consumers of such code to know the appropriate extension methods. This can be error-prone and confusing for developers; a large number of questions from new users of a particular library often revolves around setup.

Sienar tries to simplify the process for new users by implementing an interface-based plugin system. Developers can now export all the code needed to make their application work with a single plugin class, which can then be consumed by developers with a single method call. This system isn't *better* (indeed, it's marginally more complex for plugin developers), but it solves one of the problems Sienar aims to solve, which is easily distributing application code for reuse. The Sienar system allows developers to distribute plugins directly to end users as `.dll` files, which can be scanned at application startup for exported plugin implementations.