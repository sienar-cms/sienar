---
pageTitle: Plugins
blurb: "An overview of the Sienar plugin system"
pageNumber: 3
tags:
  - introduction
---

# Plugins

Sienar offers plugin-like functionality via the [ISienarPlugin](/devs/api/ISienarServerStartupPlugin) interface. This interface defines methods and properties that Sienar uses to let your plugin tap into specific parts of Sienar's functionality.

There are two interfaces that plugins can implement, depending on what the plugin needs to do. Plugins that need to add services to the DI container or set up middleware should implement `ISienarServerStartupPlugin`. Plugins that need to add pages to the app should implement `ISienarPlugin`.

## Plugins on app startup

The `ISienarServerStartupPlugin` interface is used to control the application startup process. An `ISienarServerStartupPlugin` can be registered directly to the app with the `SienarServerAppBuilder.AddStartupPlugin()` extension method. An `ISienarPlugin` can assign a `Type` to its `ISienarPlugin.StartupPlugin` property; if assigned, that `Type` should be a class implementing `ISienarServerStartupPlugin`.

`ISienarServerStartupPlugin.SetupDependencies()` is called immediately when a startup plugin is registered, and is used primarily to register services in the DI container. It receives an instance of `WebApplicationBuilder` as its only argument, so you can register services, add configuration, or do anything else you can do with a `WebApplicationBuilder`.

`ISienarServerStartupPlugin.SetupApp()` is called after the `WebApplicationBuilder` is built. It receives the `WebApplication` as its only argument. Each plugin's implementation of this method is called in the order the plugin was registered, so if you apply middleware to the `WebApplication`, you need to be aware of the order you're adding your plugins.

## Plugins on request execution

The `ISienarPlugin` interface is used to control page registration and other Sienar providers like menu items. It's added to the DI container as a scoped service, so a new instance is created on each page load.

### `ISienarPlugin.ShouldExecute()`

On each request, Sienar will call each plugin's `ISienarPlugin.ShouldExecute()` method, which determines if the plugin actually executes on a request. This allows plugins to decide if they want to only execute on certain requests, which allows for the creation of "sub-apps" that execute at different endpoints. For example, the `SienarBlazorPlugin` executes on every single request because it adds core CSS/JS files for Sienar and MudBlazor, while the `SienarCmsPlugin` only executes if the current request URL starts with `/dashboard`, which effectively creates a sub-app at the `/dashboard` URL.

`ISienarPlugin.ShouldExecute()` doesn't receive any arguments. Instead, developers should request services from the DI container to decide whether the plugin should execute. Generally, plugins can use a combination of the `HttpContext` and the `IPluginExecutionTracker` to decide whether a plugin should execute. Developers can also use `IPluginExecutionTracker` to create a default app that should execute if no other sup-apps have executed. Sub-apps are discussed in greater detail in a later article. For more information on how this works, see the [sub-app guide](/devs/guides/sub-apps).

### `ISienarPlugin.Execute()`

If `ISienarPlugin.ShouldExecute()` returns `true`, then the plugin's `Execute()` method is called. Any providers that a plugin needs to tap into should be requested from DI by the plugin's class, and any changes to providers need to be applied within `Execute()` (or a method called by `Execute()`).

For more information on which plugin providers can be used to hook into Sienar's functionality, see the [guide on providers](/devs/guides/plugin-providers).