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

### `Execute()`

```csharp
/// <summary>
/// Executes a plugin for the current request
/// </summary>
void Execute();
```

This method is used by Sienar middleware to execute a plugin. This method will actually be run twice for Blazor Server apps with prerendering enabled - once via middleware on the initial request, and again in the `<App>` component once the circuit has been initialized.

### `ShouldExecute()`

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