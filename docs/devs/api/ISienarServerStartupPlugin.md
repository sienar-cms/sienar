---
pageTitle: ISienarServerStartupPlugin
blurb: "Documentation for the ISienarServerStartupPlugin interface"
tags:
  - api
---

# ISienarServerStartupPlugin interface

Some plugins don't need to execute on each request, but do need to either add services to the DI container or add middleware to the app. Such plugins should implement `ISienarServerStartupPlugin`. This interface is also used by `ISienarPlugin` implementations to add services or middleware to the app.

## Instance methods

### SetupApp

```csharp
/// <summary>
/// Performs operations against the application's <see cref="WebApplication"/>
/// </summary>
/// <param name="app">the application's underlying <see cref="WebApplication"/></param>
void SetupApp(WebApplication app);
```

This method is used by plugins to configure the middleware pipeline. Most plugins probably won't have to do anything here, so a stub implementation should do fine. If your plugin defines a URL slug there a whole new application should be served, you should call `IEndpointRouteBuilder.MapFallbackToPage()` here. For example, `SienarCmsPlugin` only serves its pages from `/dashboard`, so it calls `app.MapFallbackToPage("/dashboard/{**segment}", "/_Host")`. You can either set up your own fallback page in your app, or if you want to rely on the Sienar system, you can fall back to `/_Host`, which comes with Sienar and includes all the regular Sienar page-level functionality right out of the box.

### SetupDependencies

```csharp
/// <summary>
/// Performs operations against the application's <see cref="WebApplicationBuilder"/>
/// </summary>
/// <param name="builder">the application's underlying <see cref="WebApplicationBuilder"/></param>
void SetupDependencies(WebApplicationBuilder builder);
```

This method is used by plugins to do any initial setup, such as adding services to the DI container. This method accepts the ASP.NET `WebApplicationBuilder` as its only argument, so if you can do it in plain ASP.NET, you can do it here. Don't build the builder though! Sienar handles the build process, so just add your plugin's configuration here and let Sienar do the rest.

## Instance properties

### PluginData

```csharp
/// <summary>
/// Optionally adds plugin data to the Sienar app
/// </summary>
/// <remarks>
/// This property is only used when directly adding a startup plugin to the app builder. When adding a plugin using the <c>SienarServerAppBuilder.AddPlugin()</c> method, the <see cref="ISienarPlugin"/>'s plugin data is used instead. 
/// </remarks>
PluginData? PluginData => null;
```

This property is used by Sienar to add a `PluginData` to the plugin container if a plugin implements `ISienarServerStartupPlugin` *instead of* `ISienarPlugin`. If an instance of `ISienarServerStartupPlugin` is registered through an associated `ISienarPlugin`, then its `PluginData` value will not be used. Unlike `ISienarPlugin.PluginData`, this property is optional.