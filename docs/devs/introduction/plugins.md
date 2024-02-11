---
pageTitle: Plugins
blurb: "An overview of the Sienar plugin system"
pageNumber: 3
tags:
  - introduction
---

# Plugins

Sienar offers plugin-like functionality via the [ISienarPlugin](/devs/api/ISienarServerStartupPlugin) interface. This interface defines a number of methods and properties that Sienar uses to let your plugin tap into specific parts of Sienar's functionality.

There are two interfaces that plugins can implement, depending on what the plugin needs to do. Plugins that need to add services to the DI container or set up middleware should implement `ISienarServerStartupPlugin`. Plugins that need to add pages to the app should implement `ISienarPlugin`.

## Plugins on app startup

The `ISienarServerStartupPlugin` interface is used to control the application startup process. An `ISienarServerStartupPlugin` can be registered directly to the app with the `SienarServerAppBuilder.AddStartupPlugin()` extension method. An `ISienarPlugin` can assign a `Type` to its `ISienarPlugin.StartupPlugin` property; if assigned, that `Type` should be a class implementing `ISienarServerStartupPlugin`.

`ISienarServerStartupPlugin.SetupDependencies()` is called immediately when a startup plugin is registered, and is used primarily to register services in the DI container. It receives an instance of `WebApplicationBuilder` as its only argument, so you can register services, add configuration, or do anything else you can do with a `WebApplicationBuilder`.

`ISienarServerStartupPlugin.SetupApp()` is called after the `WebApplicationBuilder` is built. It receives the `WebApplication` as its only argument. Each plugin's implementation of this method is called in the order the plugin was registered, so if you apply middleware to the `WebApplication`, you need to be aware of the order you're adding your plugins.

## Plugins on request execution

The `ISienarPlugin` interface is used to control page registration and other Sienar providers like menu items. It's added to the DI container as a scoped service, so a new instance is created on each page load.

On each request, Sienar will call each plugin's `ISienarPlugin.ShouldExecute()` method, which determines if the plugin actually executes on a request. This allows plugins to decide if they want to only execute on certain requests, which allows for the creation of "sub-apps" that execute at different endpoints. For example, the `SienarBlazorPlugin` executes on every single request because it adds core CSS/JS files for Sienar and MudBlazor, while the `SienarCmsPlugin` only executes if the current request URL starts with `/dashboard`, which effectively creates a sub-app at the `/dashboard` URL.

`ISienarPlugin.ShouldExecute()` doesn't receive any arguments. Instead, developers should request services from the DI container to decide whether the plugin should execute. Generally, plugins can use a combination of the `HttpContext` and the `IPluginExecutionTracker` to decide whether a plugin should execute. Developers can also use `IPluginExecutionTracker` to create a default app that should execute if no other sup-apps have executed. Sub-apps are discussed in greater detail in a later article. For more information on how this works, see the [sub-app guide](/devs/guides/sub-apps).

## Plugin methods

Because Sienar uses Blazor Server, executing a plugin on each "request" actually executes the plugin for the duration of a single user session (a single page load). For this reason, the remaining plugin methods are used to set up scoped services that provide core Sienar functionality. These services are scoped to enable Sienar's goal of supporting sub-apps at different URIs within your app.

`ISienarPlugin.SetupComponents()` is used to configure replaceable parts of the Sienar UI. This method receives an `IComponentProvider` as its only argument. The `IComponentProvider` has properties that represent these replaceable parts of the Sienar UI. For example, `IComponentProvider.AppComponent` is used to define that application's top-level `&lt;App&gt;` component. In Sienar apps, this component is provided for you automatically, but if you want to use your own, you can.

`ISienarPlugin.SetupDashboard()` is used to set up named dashboard categories with a series of links in each category, similar to the cPanel dashboard. This method receives an `IMenuProvider` as its only argument, which is used to set up different dashboard categories and add links to those categories. The `IMenuProvider` used here is a keyed service that is separate from the `IMenuProvider` supplied to `ISienarPlugin.SetupMenu()`, so you can safely use the same category and menu names with different links if you choose.

`ISienarPlugin.SetupMenu()` is used to set up named menus with a series of links in each menu. This method receives an `IMenuProvider` as its only argument, which is used to set up different named menus and add links to those named menus. The `IMenuProvider` used here is a keyed service that is separate from the `IMenuProvider` supplied to `ISienarPlugin.SetupDashboard()`, so you can safely use the same category and menu names with different links if you choose.

`ISienarPlugin.SetupRoutableComponents()` is used to let Sienar know that assemblies contain routable components. The assemblies registered here are supplied to the Blazor `&lt;Router&gt;` component.

`ISienarPlugin.SetupScripts()` is used to enqueue Javascript files needed by your plugin. Currently, all Javascript files are supplied at the end of the HTML document, regardless of configuration. However, in the future, this may change. The `IScriptProvider`, which is this method's only argument, supports enqueueing Javascript files with a number of different configuration options, including whether the file should be a regular Javascript file or an ES module, whether the file should be `async` or `defer`, etc.

`ISienarPlugin.SetupStyles()` is used to enqueue CSS files needed by your plugin. It works much the same way as `ISienarPlugin.SetupScripts()`, but because CSS and JS files support slightly different configurations, these are separate providers.

You can read more in the <MudLink Href="@Urls.Api.ISienarPlugin">ISienarPlugin documentation</MudLink>.