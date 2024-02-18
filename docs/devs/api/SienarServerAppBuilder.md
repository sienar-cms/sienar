---
pageTitle: SienarServerAppBuilder
blurb: "Documentation for the SienarServerAppBuilder class"
tags:
  - api
---

# SienarServerAppBuilder class

A Sienar app begins with `SienarServerAppBuilder` in `Program.cs`.

## Static methods

### Create

```csharp
/// <summary>
/// Creates a new <see cref="SienarServerAppBuilder"/> and registers core Sienar services on its service collection
/// </summary>
/// <param name="args">the runtime arguments supplied to <c>Program.Main()</c></param>
/// <returns>the new <see cref="SienarServerAppBuilder"/></returns>
public static SienarServerAppBuilder Create(string[] args);
```

This static method creates a new `SienarServerAppBuilder`. Internally, it creates a new instance of [WebApplication](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.webapplication?view=aspnetcore-8.0). It then adds several core Sienar utilities that are necessary for pretty much every type of Sienar app. The `WebApplication` is then stored on the `SienarServerAppBuilder`, which is returned.


### Create

```csharp
/// <summary>
/// Creates a new <see cref="SienarServerAppBuilder"/> and registers a <see cref="TContext"/> using the provided options
/// </summary>
/// <param name="args">the runtime arguments supplied to <c>Program.Main()</c></param>
/// <param name="dbContextOptionsConfigurer">an action to figure the <see cref="DbContextOptionsBuilder{TContext}"/></param>
/// <param name="dbContextLifetime">the service lifetime of the <see cref="TContext"/></param>
/// <param name="dbContextOptionsLifetime">the service lifetime of the <see cref="DbContextOptions{TContext}"/></param>
/// <typeparam name="TContext">the type of the <see cref="DbContext"/></typeparam>
/// <returns>the new <see cref="SienarServerAppBuilder"/></returns>
public static SienarServerAppBuilder Create<TContext>(
	string[] args,
	Action<DbContextOptionsBuilder>? dbContextOptionsConfigurer = null,
	ServiceLifetime dbContextLifetime = ServiceLifetime.Scoped,
	ServiceLifetime dbContextOptionsLifetime = ServiceLifetime.Scoped)
	where TContext : DbContext;
```

This method creates a `SienarServerAppBuilder` using the same rules as the first constructor. Additionally, it adds a `TContext` to the application services. This `TContext` is registered both as its own type *and* as `DbContext`, which is a type that is requested directly by Sienar's implementation types. The last three arguments to this method mirror the arguments to `IServiceCollection.AddDbContext(...)` because internally, this is how Sienar adds your `DbContext` to the service collection.

## Instance methods

### Build

```csharp
/// <summary>
/// Builds the final <see cref="WebApplication"/> and returns it
/// </summary>
/// <returns>the new <see cref="WebApplication"/></returns>
public virtual WebApplication Build()
```

This method is the last method you can call with a `SienarServerAppBuilder`. It adds any remaining services that are needed to the service collection (for example, it creates the `ThemeState`, which can't be created initially because the developer can use their own `MudTheme`), then builds the application, configures plugins and middleware, and returns the `WebApplication` for further consumption by the developer.

## Extension methods

### AddPlugin

```csharp
/// <summary>
/// Adds a plugin to the Sienar app and registers its services in the service collection
/// </summary>
/// <param name="self">the Sienar app builder</param>
/// <typeparam name="TPlugin">the type of the plugin</typeparam>
/// <returns>the Sienar app builder</returns>
public static SienarServerAppBuilder AddPlugin<TPlugin>(
	this SienarServerAppBuilder self)
	where TPlugin : class, ISienarPlugin;
```

This method is used to add plugins to the Sienar app. It adds the plugin to the service collection as a scoped service as a `ISienarPlugin`. It also registers any services or middleware if `ISienarPlugin.StartupPlugin` returns a `Type` that implements `ISienarServerStartupPlugin`.

### AddStartupPlugin

```csharp
/// <summary>
/// Adds an instance of <see cref="ISienarServerStartupPlugin"/> to the Sienar app
/// </summary>
/// <param name="self">the Sienar app builder</param>
/// <param name="plugin">an instance of the plugin to add</param>
/// <returns>the Sienar app builder</returns>
public static SienarServerAppBuilder AddStartupPlugin(
	this SienarServerAppBuilder self,
	ISienarServerStartupPlugin plugin)
```

This overload accepts an instance of `ISienarServerStartupPlugin` and adds its services and middlewares to the Sienar app. If `ISienarServerStartupPlugin.PluginData` is not `null`, it is added to the plugin data provider.

### AddStartupPlugin

```csharp
/// <summary>
/// Adds an <see cref="ISienarServerStartupPlugin"/> to the Sienar app
/// </summary>
/// <param name="self">the Sienar app builder</param>
/// <typeparam name="TPlugin">the type of the plugin to add</typeparam>
/// <returns>the Sienar app builder</returns>
public static SienarServerAppBuilder AddStartupPlugin<TPlugin>(
	this SienarServerAppBuilder self)
	where TPlugin : ISienarServerStartupPlugin, new();
```

This overload accepts a type parameter indicating the type of the `ISienarServerStartupPlugin` and calls the non-generic overload of this method with a new instance of the type indicated. In order to use this overload, your `ISienarServerStartupPlugin` must provide a default constructor.

### ConfigureTheme

```csharp
/// <summary>
/// Registers a custom <see cref="MudTheme"/> for use in Sienar's <see cref="ThemeState"/>
/// </summary>
/// <param name="self">the Sienar app builder</param>
/// <param name="theme">the <see cref="MudTheme"/> to use</param>
/// <param name="isDarkMode">whether the theme represents dark mode or not</param>
/// <typeparam name="TBuilder">the type of the Sienar app builder</typeparam>
/// <returns>the Sienar app builder</returns>
public static TBuilder ConfigureTheme<TBuilder>(
	this TBuilder self,
	MudTheme theme,
	bool isDarkMode = false)
	where TBuilder : SienarServerAppBuilder;
```

This overload is used to tell Sienar to use a specific instance of a custom `MudTheme`. Sienar uses a `ThemeState` state provider, and the theme provided here is used to construct the `ThemeState`. If no custom theme is configured, Sienar uses a new blank `MudTheme` with no special configuration.

### ConfigureTheme

```csharp
/// <summary>
/// Registers a custom <see cref="MudTheme"/> for use in Sienar's <see cref="ThemeState"/>
/// </summary>
/// <param name="self">the Sienar app builder</param>
/// <param name="isDarkMode">whether the theme represents dark mode or not</param>
/// <typeparam name="TTheme">the type of the theme to register</typeparam>
/// <returns>the Sienar app builder</returns>
public static SienarServerAppBuilder ConfigureTheme<TTheme>(
	this SienarServerAppBuilder self,
	bool isDarkMode = false)
	where TTheme : MudTheme, new();
```

This overload accepts a type parameter indicating the type of the theme to register. It creates a new instance of the theme and calls the first overload of this method with the theme instance. In order to use this overload, your `MudTheme` class must provide a default constructor.