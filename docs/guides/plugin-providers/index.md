# Customizing Sienar using plugin providers

Sienar provides plugins with a number of "providers", which are singleton services that help configure specific parts of the Sienar application at startup. Providers are singleton because the configurations they provide should not change across page visits. The following guides show you how different providers can be used to customize Sienar apps.

### Example code boilerplate

To cut down on repetition, every example is assumed to be running in the following boilerplate:

```csharp
using Microsoft.AspNetCore.Builder; // Imports WebApplication
using Sienar.Extensions; // Imports SienarWebAppBuilder.BuildBlazor() extension method
using Sienar.Infrastructure; // Imports SienarWebAppBuilder
using Sienar.Infrastructure.Plugins; // Imports SienarCmsBlazorPlugin

await SienarWebAppBuilder
	.Create(args, typeof(Program).Assembly)
	.AddPlugin<SienarCmsBlazorPlugin>()
	.SetupApp(Configure)
	.BuildBlazor()
	.RunAsync();

void Configure(WebApplication app)
{
}
```

Then, each example will show the `void Configure(WebApplication)` method.

## Available providers

Use `IComponentProvider` to [override parts of the Sienar UI](overriding-ui-components.md).

Use `IMenuProvider` to [add menu items](adding-menu-items.md) to an app or sub-app.

Use `IDashboardProvider` to [add dashboard items](adding-dashboard-items.md) to an app or sub-app.

Use `IRoutableAssemblyProvider` to [add routable pages](adding-routable-pages.md) to an app or sub-app.

Use `IScriptProvider` to [add JS files](adding-js.md) to an app or sub-app.

Use `IStyleProvider` to [add CSS files](adding-css.md) to an app or sub-app.

**NOTE**: While it's possible to configure providers anywhere, they're only intended to be configured at application startup. The behavior of configuring plugin providers after application startup is undefined, and will likely result in unexpected functionality. For that reason, every example will show you how to configure the given provider via the `SienarWebAppBuilder.SetupApp()` method. You can also configure providers from plugins in the `IWebPlugin.SetupApp()` method or `IDesktopPlugin.SetupApp()` method.