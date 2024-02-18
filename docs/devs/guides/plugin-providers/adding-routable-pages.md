---
pageTitle: "Adding routable pages"
blurb: "A guide to adding routable pages with IRoutableAssemblyProvider"
pageNumber: 5
tags:
  - plugin-providers
---

# Adding routable pages with `IRoutableAssemblyProvider`

Sienar enables developers to add their routable pages to apps and sub-apps at will. Routable pages can be added by adding a reference to their containing `Assembly` to the `IRoutableAssemblyProvider`.

**NOTE**: While it's possible to configure the `IRoutableAssemblyProvider` anywhere, it's only intended to be configured via a plugin. The behavior of configuring plugin providers outside a plugin is undefined, and will likely result in unexpected functionality. For that reason, every example will show you how to configure the `IRoutableAssemblyProvider` via the `ISienarPlugin.SetupRoutableAssemblies()` method.

## Overview

Razor Pages and Blazor add routable pages in fundamentally different ways. In a Razor Pages app, pages are discovered automatically in every assembly. However, in Blazor, you need to manually add assemblies to your `<Router>` in `App.razor`:

```razor
@* App.razor *@

<Router
	AppAssembly="@typeof(App).Assembly"
	AdditionalAssemblies="@([typeof(Component1).Assembly, typeof(Component2).Assembly])">
@*...*@
</Router>
```

This `Assembly`-based model allows developers to unambiguously determine which assemblies they want to route from. Sienar exposes the `IRoutableAssemblyProvider` to give plugin developers the ability to hook into this system and register routable assemblies with the `<Router>`.

### `IRoutableAssemblyProvider`

The `IRoutableAssemblyProvider` is the container for routable assemblies. It's backed by a `List<Assembly>` and contains a single public method, `Add(Assembly)`. The `Add()` method is fluent, so multiple assemblies can be added via method chaining.

The Sienar-provided `SienarApp.razor` directly uses the `Assembly` entries from the `IRoutableAssemblyProvider`; the first `Assembly` is used as the `Router.AppAssembly` parameter, while the remaining assemblies are used as the `Router.AdditionalAssemblies` parameter.

**NOTE**: If no plugin registers a routable assembly, Sienar's `SienarApp.razor` file will throw a runtime exception. This is because `Router.AppAssembly` is a required value, so Sienar does not check if at least one routable assembly has been registered - instead, it accesses the first assembly directly with an index accessor. If no routable assembly is registered, an `ArgumentOutOfRangeException` will be thrown.

## Examples

In each of our examples, we will add fictional assemblies to the routable assembly container.

### Example 1: Adding the plugin's assembly as a routable assembly

To add the plugin's assembly as a routable assembly, you can either use the `typeof(TPlugin)` or `this.GetType()`:

```csharp
using Sienar.Infrastructure.Plugins; // Imports ISienarPlugin
using MyPlugin; // Imports CustomPlugin

public class CustomPlugin : ISienarPlugin
{
	public void SetupRoutableAssemblies(IRoutableAssemblyProvider assemblyProvider)
	{
		// Using typeof
		assemblyProvider.Add(typeof(MyPlugin).Assembly);

		// Using GetType()
		assemblyProvider.Add(this.GetType().Assembly);
	}
}
```

### Example 2: Adding an external assembly as a routable assembly

To add a separate assembly as a routable assembly, you reference the type of any class in that assembly and add its `Assembly` to the assembly provider instead of the plugin's `Assembly`:

```csharp
using Sienar.Infrastructure.Plugins; // Imports ISienarPlugin
using MyPlugin; // Imports CustomPlugin
using RoutableComponents; // Imports Component1

public class CustomPlugin : ISienarPlugin
{
	public void SetupRoutableAssemblies(IRoutableAssemblyProvider assemblyProvider)
	{
		// Use typeof since there is no instance
		assemblyProvider.Add(typeof(Component1).Assembly);
	}
}
```

### Example 3: Adding multiple assemblies as routable assemblies

to add multiple assemblies, you use the techniques in the previous two examples to get references to each `Assembly`, then add them each in turn. `IRoutableAssemblyProvider` is fluent, so you can chain calls to `IRoutableAssemblyProvider.Add()` if you wish.

```csharp
using Sienar.Infrastructure.Plugins; // Imports ISienarPlugin
using MyPlugin; // Imports CustomPlugin
using RoutableComponents; // Imports Component1

public class CustomPlugin : ISienarPlugin
{
	public void SetupRoutableAssemblies(IRoutableAssemblyProvider assemblyProvider)
	{
		assemblyProvider
			.Add(typeof(CustomPlugin).Assembly)
			.Add(typeof(Component1).Assembly);
	}
}
```