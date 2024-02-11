---
pageTitle: ISienarPlugin
blurb: "Documentation for the ISienarPlugin interface"
tags:
  - api
---

# ISienarPlugin interface

`ISienarPlugin` is used directly by Sienar to encapsulate behaviors that need to be repeated on each full page load. These behaviors use various data providers, but they should also generally execute either together or not at all, so they are grouped together on a single interface.

Each instance method has a default implementation that does nothing (with the exception of `ISienarPlugin.ShouldExecute()`, which returns `true` by default). This allows developers to only implement the behavior they need without needing to worry about adding several empty methods just to satisfy the compiler.

## Instance methods

### SetupComponents

```csharp
/// <summary>
/// Configures various components to replace specific parts of the Sienar UI
/// </summary>
/// <param name="componentProvider">the <see cref="IComponentProvider"/></param>
void SetupComponents(IComponentProvider componentProvider);
```

This method uses the `IComponentProvider`, which is a container that includes references to components that should be used in various parts of the Sienar UI. More information is available in the guide at the end of this README, under `Customization > Using configuration providers > IComponentProvider`.

### SetupDashboard

```csharp
/// <summary>
/// Configures dashboard items to be registered for the current user session
/// </summary>
/// <param name="dashboardProvider">the <see cref="IMenuProvider"/> containing dashboard item definitions</param>
void SetupDashboard(IMenuProvider dashboardProvider);
```

This method uses a version of the `IMenuProvider` keyed to provide dashboard items instead of menu items. `SetupDashboard()` supports adding links to a series of sections, similar to the cPanel UI. Dashboards support a limited set of features compared to menus. More information is available in the guide at the end of this README, under `Customization > Using configuration providers > IMenuProvider`.

### SetupMenu

```csharp
/// <summary>
/// Configures menu items to be registered for the current user session
/// </summary>
/// <param name="menuProvider">the <see cref="IMenuProvider"/> containing menu item definitions</param>
void SetupMenu(IMenuProvider menuProvider);
```

This method uses a version of the `IMenuProvider` keyed to provide menu itmes. This method allows your plugin to add items to various named menus of different pages in an app. More information is available in the guide at the end of this README, under `Customization > Using configuration providers > IMenuProvider`.

### SetupRoutableAssemblies

```csharp
/// <summary>
/// Configures routable assemblies
/// </summary>
/// <param name="routableAssemblyProvider">the <see cref="IRoutableAssemblyProvider"/></param>
void SetupRoutableAssemblies(IRoutableAssemblyProvider routableAssemblyProvider);
```

This method uses the `IRoutableAssemblyProvider` to register assemblies that contain routable Blazor components. The assemblies registered here are supplied to the Blazor router.

### SetupScripts

```csharp
/// <summary>
/// Configures scripts to be loaded with the current user session
/// </summary>
/// <param name="scriptProvider">the <see cref="IScriptProvider"/></param>
void SetupScripts(IScriptProvider scriptProvider);
```

This method uses the `IScriptProvider` to provide JavaScript inclusion on webpages. The `IScriptProvider` supports most scenarios for including JavaScript on a webpage, including whether the script should be `async`, whether the script should `defer`, whether the script is a module or a regular JavaScript file, etc. Simply omit any value you don't need, and it won't appear in the HTML. More information is available in the guide at the end of this README, under `Customization > Using configuration providers > IScriptProvider`.

### SetupStyles

```csharp
/// <summary>
/// Configures stylesheets to be loaded with the current user session
/// </summary>
/// <param name="styleProvider">the <see cref="IStyleProvider"/></param>
void SetupStyles(IStyleProvider styleProvider);
```

This method uses the `IStyleProvider` to provide CSS inclusion on webpages. For example, the `SienarBlazorPlugin` uses the `IStyleProvider` to load MudBlazor's CSS on every page of a Sienar app. The `IStyleProvider` has support for HTML features of including CSS files, including things like the `crossorigin` and `integrity` attributes. Simply omit any value you don't need, and it won't appear in the HTML. More information is available in the guide at the end of this README, under `Customization > Using configuration providers > IStyleProvider`.

### ShouldExecute

```csharp
/// <summary>
/// Determines whether the plugin should execute for the current request
/// </summary>
/// <returns>whether the plugin should execute</returns>
bool ShouldExecute();
```

This method is used by Sienar middleware to determine if the plugin should execute on each request. (Because Blazor Server treats each "request" as an entire user session, this method will only run on each full page load.) It doesn't accept any arguments, but because plugins are scoped, your plugin can request any services from DI via constructor injection like usual.

## Instance properties

### PluginData

```csharp
/// <summary>
/// Plugin data for the current plugin
/// </summary>
PluginData PluginData { get; }
```

This property contains an instance of the `PluginData` class, which provides users with information about the plugin, such as the name of the plugin, the name of the author, the plugin's website, and more.

## Static properties

### StartupPlugin

```csharp
/// <summary>
/// Provides the <see cref="Type"/> of the startup plugin to use for this plugin, if any
/// </summary>
static virtual Type? StartupPlugin => null;
```

This property constains a static reference to the `Type` that should be used to set up this plugin's services and middleware. This property is optional and its default value is `null`, but if you do supply a `Type`, it needs to implement `ISienarServerStartupPlugin`.