---
pageTitle: "Adding CSS"
blurb: "A guide to adding CSS files with IStyleProvider"
pageNumber: 6
tags:
  - plugin-providers
---

# Adding CSS with `IStyleProvider`

Sienar enables developers to add CSS for their plugins with the `IStyleProvider` interface. This provider contains references to `StyleResource` objects, which are rendered in the order they were registered in the provider.

## Overview

### `IStyleProvider`

The `IStyleProvider` is a container for `StyleResource` instances. It's backed by a `List<StyleResource>` and has a single public method, `Add(StyleResource)`. The `Add()` method is fluent, so multiple `StyleResource` instances can be added via method chaining.

### `StyleResource`

The `StyleResource` class contains the data needed to construct a `<link>` tag in HTML. It only supports external CSS stylesheets; internal CSS stylesheets and other `<link>` resources like icons are not supported. It can be used to link styles from your plugin's `wwwroot` folder, your app's `wwwroot` folder, CDNs, or other third-party sources of CSS stylesheets. As long as the provided `StyleResource.Href` value is a resolveable web address, the CSS will be included. For more information on the `StyleResource` class, see its [Api documentation](/devs/api/StyleResource);

## Examples

In each of our examples, we will add a single style to the `IStyleProvider`. Remember that you can chain `Add()` calls together because `IStyleProvider` exposes a fluent API.

It is also worth noting that `StyleResource` has an implicit cast operator from `string`, so if you call `Add()` with a `string` representing the URL of a stylesheet, it will implicitly convert that string to a `StyleResource` with its `Href` property set to the value of the string. This simplifies adding simple stylesheets that don't require extra options such as `StyleResource.CrossOriginMode`.

### Example 1: Adding a stylesheet from your app's `wwwroot` directory

Adding a stylesheet from your app's `wwwroot` directory is as simple as adding a `StyleResource` with the URL of the stylesheet as its `Href` property. If your stylesheet is at `/wwwroot/css/myStyle.css`, your code will look like this:

```csharp
public void Execute()
{
	// Explicit ScriptResource
	_provider.Add(new StyleResource { Href = "/css/myStyle.css");

	// Implicit cast from string
	_provider.Add("/css/myStyle.css");
}
```

The generated link tag will look like this:

```html
<link rel="stylesheet" href="/css/myStyle.css"/>
```

### Example 2: Adding a stylesheet from your plugin's `wwwroot` directory

Adding a stylesheet from your plugin's `wwwroot` directory is similar to adding a stylesheet from your app's `wwwroot` directory, but you need to prepend `/_content/<plugin-assembly>` to the path. If your stylesheet is at `<plugin-root>/wwwroot/css/myStyle.css` and your plugin's assembly name is `CustomPlugin`, your code will look like this:

```csharp
public void Execute()
{
	// Explicit ScriptResource
	_provider.Add(new StyleResource { Href = "/_content/CustomPlugin/css/myStyle.css");

	// Implicit cast from string
	_provider.Add("/_content/CustomPlugin/css/myStyle.css");
}
```

The generated link tag will look like this:

```html
<link rel="stylesheet" href="/_content/CustomPlugin/css/myStyle.css"/>
```

### Complete example: Adding Bootstrap from a CDN

This is a more complete example that shows you how to use multiple properties together to create a complex link tag. We want to include Bootstrap from CDNJS for security reasons because it serves Bootstrap via HTTPS and includes a file hash to ensure no one has tampered with our file en route. To include Bootstrap v5.3.2 from CDNJS, your code will look like this:

```csharp
public void Execute()
{
	// Explicit ScriptResource
	_provider.Add(
		new StyleResource
		{
			Href = "https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.2/css/bootstrap.min.css",
			CrossOriginMode = CrossOriginMode.Anonymous,
			ReferrerPolicy = ReferrerPolicy.NoReferrer,
			Integrity = "sha512-b2QcS5SsA8tZodcDtGRELiGv5SaKSk1vDHDaQRda0htPYWZ6046lr3kJ5bAAQdpV2mmA/4v0wQF9MyU6/pDIAg=="
		});
}
```

The generated link tag will look like this:

```html
<link
	rel="stylesheet"
	href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.2/css/bootstrap.min.css"
	crossorigin="anonymous"
	referrerpolicy="no-referrer"
	integrity="sha512-b2QcS5SsA8tZodcDtGRELiGv5SaKSk1vDHDaQRda0htPYWZ6046lr3kJ5bAAQdpV2mmA/4v0wQF9MyU6/pDIAg=="/>
```