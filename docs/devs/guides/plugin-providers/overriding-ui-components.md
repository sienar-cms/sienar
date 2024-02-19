---
pageTitle: "Overriding UI components"
blurb: "A guide to overriding UI components with IComponentProvider"
pageNumber: 2
tags:
  - plugin-providers
---

# Overriding UI components with IComponentProvider

Sienar does most of the base layout work for you with the `SienarLayoutBase` component. You can use your own layout class if you want. You can even override the default layout (more on that in a moment). But if you want to continue using `SienarLayoutBase` and its derived components, you can still customize parts of the layout using the `IComponentProvider`.

## Overriding the root `<App>` component

### Background

Every Blazor app has a root component. In a new project created by the .NET CLI, this component is called `<App>`. Its primary role is to configure the router and add any cascading state that will be needed by the entire app.

### Sienar-specific considerations

Because of how Sienar's plugin system works, any `<App>` component used in a Sienar app also needs to do extra work to load plugins on initialization. This is because in a pre-rendered Blazor Server app, your Blazor code is actually run *twice*: once during pre-render, and again after the circuit is initialized. This is a problem for scoped services because in this double-execution model, your code is run in a different DI scope for the second pass - and therefore, your plugins are no longer initialized and your plugin providers are no longer configured. Any root `<App>` component in a Sienar app needs to re-configure plugins if they haven't already been configured. The easiest way to do this is to inherit from `SienarBlazorServerApp`, which does the plugin re-initialization in its `OnInitialized()` method. You can feel free to provide your own UI, just make sure that you call `SienarBlazorServerApp.OnInitialized()`.

If you would prefer to provide your own custom base `<App>` component without inheriting from `SienarBlazorServerApp`, take a look at the [SienarBlazorServerApp source]() so you can see what it's doing and how. One thing to notice is that it has a `string[] PluginNames` parameter - this is how Sienar lets the second pass know which plugins were configured during pre-render. This parameter is used by `/_Host.cshtml`, so it has to be present if you use `/_Host.cshtml` as your fallback page. If you provide your own fallback page, you can safely omit the `PluginNames` parameter, but you will need to either determine which plugins to re-initialize, or else re-initialize every plugin.

One benefit of providing your own `<App>` component is that you won't need to use most of the other `IComponentProvider` overrides because you can just configure them directly in your own `<App>` code.

### Overriding

After you have a base `<App>` component, you can tell Sienar to use it by setting the `App` property on `IComponentProvider`:

```csharp
public void Execute()
{
    _provider.App = typeof(YourAppComponent);
}
```

**NOTE**: Sienar configures its default value for `IComponentProvider.AppComponent` in the `SienarBlazorPlugin`, so if you override this value, your plugin needs to be registered after `SienarBlazorPlugin`.

## Overriding the default layout

### Background

Blazor allows you to set an app's default layout on the `AuthorizeRouteView.DefaultLayout` and `RouteView.DefaultLayout` parameters, depending on which route view your application uses. This saves you from needing to supply a `@layout` on every page component in your app.

### Overriding

After creating your own default layout, you can register it by setting the `DefaultLayout` property on `IComponentProvider`:

```csharp
public void Execute()
{
    _provider.DefaultLayout = typeof(YourDefaultLayout);
}
```

**NOTE**: Sienar configures its default value for `IComponentProvider.DefaultLayout` in the `SienarBlazorPlugin`, so if you override this value, your plugin needs to be registered after `SienarBlazorPlugin`.

## Adding new top-level components

### Background

You can add just about anything you want in your root `<App>` component. For example, Sienar uses a component to update a user's login status every 10 minutes. Because this component doesn't render anything, and because it needs to be present on every page regardless of layout, this component is unceremoniously placed at the top of `<SienarBlazorServerApp>`, which is Sienar's root `<App>` component. Sienar maintains a `List<Type>` that contains each component that should be placed at this top level of the app.

### Adding to the list

After creating a component you want at the top level of the app, you can register it by adding it to the `IComponentProvider.TopLevelComponents` list:

```csharp
public void Execute()
{
    _provider.TopLevelComponents.Add(typeof(YourTopLevelComponent));
}
```

**NOTE**: Sienar doesn't enable you to directly remove top-level components once they've been registered. However, `IComponentProvider.TopLevelComponents` is just a regular `List<Type>`, so if you must remove an item from the list, you can.

## Overriding the `<Authorizing>` and `<NotAuthorized>` content

### Background

In Blazor apps that use authorization, it's standard to use an `<AuthorizeRouteView>` to render your page. That component has two `RenderFragment` parameters: `<Authorizing>`, which defines content to show while Blazor waits for authorization to complete, and `<NotAuthorized>`, which defines content to show when Blazor has completed the auth process and determined that the user is unauthorized. The `SienarBlazorPlugin` sets default values for these components. The default `<NotAuthorized>` content is a component that redirects the user to the login page if they aren't logged in, or to the forbidden page if they are logged in. The default `<Authorizing>` content is this:

```razor
@* Default <Authorizing> content *@

<H1>Authorizing</H1>

<P>We are checking your authorization status. Please wait...</P>
```

### Overriding

To override the default values for these components, you should set their corresponding properties on the `IComponentProvider`:

```csharp
public void Execute()
{
	// Change <AuthorizingView>
    _provider.AuthorizingView = typeof(YourAuthorizingView);

    // Change <NotAuthorizedView>
    _provider.NotAuthorizedView = typeof(YourNotAuthorizedView);
}
```

**NOTE**: Sienar configures these default values for all routes in the `SienarBlazorPlugin`, so if you override either of these values, your plugin needs to be registered after `SienarBlazorPlugin`.

## Overriding the sidebar header and footer content

### Background

In Sienar, the `SienarLayoutBase` layout has a sidebar on the left side of the page. This sidebar contains the app's primary navigation. This layout also offers the ability to optionally add an arbitrary component at the top and bottom of the sidebar. These components don't receive any parameters, so you can create anything you want. A common use of the header area is to display a welcome message to the user if they are logged in (this is what Sienar's default header does). A common use of the footer area is to display a copyright notice. Sienar does not supply a default footer.

### Overriding the default sidebar header and footer

To override the default values for these components, set their corresponding properties on the `IComponentProvider`:

```csharp
public void Execute()
{
	// Change the sidebar header
    _provider.SidebarHeader = typeof(YourSidebarHeader);

    // Change the sidebar footer
    _provider.SidebarFooter = typeof(YourSidebarFooter);
}
```

**NOTE**: Sienar configures a default sidebar header in the `SienarCmsPlugin`, so if you want to override the sidebar header on `/dashboard` pages, you need to register your plugin after `SienarCmsPlugin`.