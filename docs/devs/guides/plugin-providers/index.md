---
pageTitle: "Customizing Sienar using plugin providers"
blurb: "A guide to customizing Sienar using plugin providers"
pageNumber: 1
tags:
  - plugin-providers
  - guides
---

# Customizing Sienar using plugin providers

Sienar provides plugins with a number of "providers", which are scoped services that help configure specific parts of the Sienar application on each request. Providers are scoped to the current request because Sienar supports having different [sub-apps](/devs/guides/sub-apps) at different endpoints, so providers need to be re-populated per page load. The following guides show you how different providers can be used to configure different Sienar services.

### Example code boilerplate

To cut down on repetition, every example is assumed to be running in the following boilerplate:

```csharp
using Sienar.Infrastructure.Plugins; // Import ISienarPlugin interface

public class CustomPlugin : ISienarPlugin
{
    // PluginData omitted

	// Here, T is the type of the provider
	private readonly T _provider;

	public CustomPlugin(T provider)
	{
		_provider = provider;
	}
}
```

Then, each example will show the `ISienarPlugin.Execute()` method.

## Available providers

Use `IComponentProvider` to [override parts of the Sienar UI](/devs/guides/plugin-providers/overriding-ui-components).

Use `IMenuProvider` to [add menu items](/devs/guides/plugin-providers/adding-menu-items) to an app or sub-app.

Use `IDashboardProvider` to [add dashboard items](/devs/guides/plugin-providers/adding-dashboard-items) to an app or sub-app.

Use `IRoutableAssemblyProvider` to [add routable pages](/devs/guides/plugin-providers/adding-routable-pages) to an app or sub-app.

Use `IScriptProvider` to [add JS files](/devs/guides/plugin-providers/adding-js) to an app or sub-app.

Use `IStyleProvider` to [add CSS files](/devs/guides/plugin-providers/adding-css) to an app or sub-app.

**NOTE**: While it's possible to configure providers anywhere, they're only intended to be configured via a plugin. The behavior of configuring plugin providers outside a plugin is undefined, and will likely result in unexpected functionality. For that reason, every example will show you how to configure the given provider via the `ISienarPlugin.Execute()` method.