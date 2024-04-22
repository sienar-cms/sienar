# Overriding UI components with IComponentProvider

Sienar does most of the base layout work for you with the `CmsLayoutBase` component. You can use your own layout class if you want. You can even override the default layout (more on that in a moment). But if you want to continue using `CmsLayoutBase` and its derived components (like `DashboardLayout`, which is the layout used on `/dashboard` pages), you can still customize parts of the layout using the `IComponentProvider`.

## Overriding the default layout

### Background

Blazor allows you to set an app's default layout on the `AuthorizeRouteView.DefaultLayout` and `RouteView.DefaultLayout` parameters, depending on which route view your application uses. This saves you from needing to supply a `@layout` on every page component in your app. Sienar supplied a value to this parameter using the `IComponentProvider.DefaultLayout` property.

### Overriding

After creating your own default layout, you can register it by setting the `DefaultLayout` property on `IComponentProvider`:

```csharp
void Configure(WebApplication app)
{
 	app.ConfigureComponents(componentProvider =>
	{
		componentProvider.DefaultLayout = typeof(YourDefaultLayout);
	});
}
```

> [!NOTE]
> Sienar configures its default value for `IComponentProvider.DefaultLayout` in the `SienarCmsBlazorPlugin` using the null-coalescing assignment operator (`??=`), so your default layout will be honored regardless of when you configure it.

## Overriding the appbar left and right content

### Background

In Sienar, the `TopNavLayoutBase` and `TopNavPaddedLayoutBase` components have an appbar at the top of the page. This appbar contains the app's primary navigation on desktop-sized layouts. It's common to want to add branding or additional menus to your app before or after your menu content, so Sienar offers properties on `IComponentProvider` to allow developers to add content to these areas.

### Overriding the appbar left and right content areas

Sienar does not provide a default value for either of these components, so if you want to use them, you must set them yourself. To provide values for these components, set their corresponding properties on the `IComponentProvider`:

```csharp
void Configure(WebApplication app)
{
	app.ConfigureComponents(componentProvider =>
	{
		// Provide the left appbar content area
		componentProvider.AppbarLeft = typeof(YourLeftContent);

		// Provide the right appbar content area
		componentProvider.AppbarRight = typeof(YourRightContent);
	});
}
```

## Overriding the sidebar header and footer content

### Background

In Sienar, the `CmsLayoutBase` layout has a sidebar on the left side of the page. This sidebar contains the app's primary navigation. This layout also offers the ability to optionally add an arbitrary component at the top and bottom of the sidebar. These components don't receive any parameters, so you can create anything you want. A common use of the header area is to display a welcome message to the user if they are logged in (this is what Sienar's default header does). A common use of the footer area is to display a copyright notice. Sienar does not supply a default footer.

### Overriding the default sidebar header and footer

To override the default values for these components, set their corresponding properties on the `IComponentProvider`:

```csharp
void Configure(WebApplication app)
{
	app.ConfigureComponents(componentProvider =>
	{
		// Change the sidebar header
		componentProvider.SidebarHeader = typeof(YourSidebarHeader);

		// Change the sidebar footer
		componentProvider.SidebarFooter = typeof(YourSidebarFooter);
	});
}
```

> [!NOTE]
> Sienar configures its default value for `IComponentProvider.SidebarHeader` in the `SienarCmsBlazorPlugin` using the null-coalescing assignment operator (`??=`), so your header will be honored regardless of when you configure it.