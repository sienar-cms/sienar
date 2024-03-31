# Adding routable pages with `IRoutableAssemblyProvider`

Sienar enables developers to add their routable pages to apps at will. Routable pages can be added by adding a reference to their containing `Assembly` to the `IRoutableAssemblyProvider`.

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

The Sienar-provided `SienarRoutes.razor` directly uses the `Assembly` entries from the `IRoutableAssemblyProvider`; the first `Assembly` is used as the `Router.AppAssembly` parameter, while the remaining assemblies are used as the `Router.AdditionalAssemblies` parameter.

**NOTE**: You do not need to add your app's entrypoint assembly to the `IRoutableAssemblyProvider` because this is done for you by Sienar. Sienar's boilerplate passes `typeof(Program).Assembly` as the second argument to `SienarWebAppBuilder.Create()`. If you define your routable components in the same assembly as `Program.cs`, this will work fine, but if you define your routable components in a separate assembly, you should provide a reference to that assembly instead.

## Examples

In each of our examples, we will add fictional assemblies to the routable assembly container.

### Example 1: Adding the plugin's assembly as a routable assembly

To add the plugin's assembly as a routable assembly, you can use `typeof(TPlugin).Assembly`:

```csharp
void Configure(WebApplication app)
{
	app.ConfigureRoutableAssemblies(provider =>
	{
		provider.Add(typeof(MyPlugin).Assembly);
	});
}
```

### Example 2: Adding an external assembly as a routable assembly

To add a separate assembly as a routable assembly, you reference the type of any class in that assembly and add its `Assembly` to the assembly provider instead of the plugin's `Assembly`:

```csharp
void Configure(WebApplication app)
{
	app.ConfigureRoutableAssemblies(provider =>
	{
		provider.Add(typeof(Component1).Assembly);
	});
}
```

### Example 3: Adding multiple assemblies as routable assemblies

to add multiple assemblies, you use the techniques in the previous two examples to get references to each `Assembly`, then add them each in turn. `IRoutableAssemblyProvider` is fluent, so you can chain calls to `IRoutableAssemblyProvider.Add()` if you wish.

```csharp
void Configure(WebApplication app)
{
	app.ConfigureRoutableAssemblies(provider =>
	{
		provider
			.Add(typeof(CustomPlugin).Assembly)
			.Add(typeof(Component1).Assembly);
	});
}
```