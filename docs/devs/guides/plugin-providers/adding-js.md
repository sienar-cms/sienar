---
pageTitle: "Adding JavaScript"
blurb: "A guide to adding JavaScript files with IScriptProvider"
pageNumber: 6
tags:
  - plugin-providers
---

# Adding JavaScript with `IScriptProvider`

Sienar enables developers to add JavaScript for their plugins with the `IScriptProvider` interface. This provider contains references to `ScriptResource` objects, which are rendered in the order they were registered in the provider.

## Overview

As a Blazor app, Sienar removes most of your need for JavaScript. However, you may wish to include JavaScript in certain circumstances, such as using an existing JavaScript library instead of reproducing the functionality in C# or handling edge cases where Blazor still requires JavaScript.

### `IScriptProvider`

The `IScriptProvider` is a container for `ScriptResource` instances. It's backed by a `List<ScriptResource>` and has a single public method, `Add(ScriptResource)`. The `Add()` method is fluent, so multiple `ScriptResource` instances can be added via method chaining.

### `ScriptResource`

The `ScriptResource` class contains the data needed to construct a `<script>` tag in HTML. It only supports external scripts; inline scripts are not supported. It can be used to link scripts from your plugin's `wwwroot` folder, your app's `wwwroot` folder, CDNs, or other third-party sources of script files. As long as the provided `ScriptResource.Src` value is a resolveable web address, the script will be included. For more information on the `ScriptResource` class, see its [API documentation](/devs/api/ScriptResource).

## Examples

In each of our examples, we will add a single script to the `IScriptProvider`. Remember that you can chain `Add()` calls together because `IScriptProvider` exposes a fluent API.

It is also worth noting that `ScriptResource` has an implicit cast operator from `string`, so if you call `Add()` with a `string` representing the URL of a script resource, it will implicitly convert that string to a `ScriptResource` with its `Src` property set to the value of the string. This simplifies adding simple JavaScript resources that don't require extra options such as `ScriptResource.IsAsync`.

### Example 1: Adding a script file from your app's `wwwroot` directory

Adding a script from your app's `wwwroot` directory is as simple as adding a `ScriptResource` with the URL of the script as its `Src` property. If your script is at `/wwwroot/js/myScript.js`, your code will look like this:

```csharp
public void Execute()
{
	// Explicit ScriptResource
	_provider.Add(new ScriptResource { Src = "/js/myScript.js");

	// Implicit cast from string
	_provider.Add("/js/myScript.js");
}
```

The generated script tag will look like this:

```html
<script src="/js/myScript.js"></script>
```

### Example 2: Adding a script file from your plugin's `wwwroot` directory

Adding a script from your plugin's `wwwroot` directory is similar to adding a script from your app's `wwwroot` directory, but you need to prepend `/_content/<plugin-assembly>` to the path. If your script is at `<plugin-root>/wwwroot/js/myScript.js` and your plugin's assembly name is `CustomPlugin`, your code will look like this:

```csharp
public void Execute()
{
	// Explicit ScriptResource
	_provider.Add(new ScriptResource { Src = "/_content/CustomPlugin/js/myScript.js");

	// Implicit cast from string
	_provider.Add("/_content/CustomPlugin/js/myScript.js");
}
```

The generated script tag will look like this:

```html
<script src="/_content/CustomPlugin/js/myScript.js"></script>
```

### Example 3: Adding Google Analytics (`async` script)

Google Analytics is included in your webpage by adding a `<script>` to your HTML. The official docs installation shows the `gtag` script being included as `async`. To do that:

```csharp
public void Execute()
{
    var gtagId = "my-gtag-id";

	_provider.Add(
		new ScriptResource
		{
			Src = $"https://www.googletagmanager.com/gtag/js?id={gtagId}",
			IsAsync = true
		});
}
```

The generated script tag will look like this:

```html
<script
	src="https://www.googletagmanager.com/gtag/js?id={gtagId}"
	async></script>
```

### Complete example: Adding Bootstrap from a CDN

This is a more complete example that shows you how to use multiple properties together to create a complex script tag. We want to include Bootstrap from CDNJS for security reasons because it serves Bootstrap via HTTPS and includes a file hash to ensure no one has tampered with our file en route. To include Bootstrap v5.3.2 from CDNJS, your code will look like this:

```csharp
public void Execute()
{
	_provider.Add(
		new ScriptResource
		{
			Src = "https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.2/js/bootstrap.min.js",
            CrossOriginMode = CrossOriginMode.Anonymous,
            ReferrerPolicy = ReferrerPolicy.NoReferrer,
            Integrity = "sha512-WW8/jxkELe2CAiE4LvQfwm1rajOS8PHasCCx+knHG0gBHt8EXxS6T6tJRTGuDQVnluuAvMxWF4j8SNFDKceLFg==",
            ShouldDefer = true
		});
}
```

The generated script tag will look like this:

```html
<script
	src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.2/js/bootstrap.min.js"
	crossorigin="anonymous"
	referrerpolicy="no-referrer"
	integrity="sha512-WW8/jxkELe2CAiE4LvQfwm1rajOS8PHasCCx+knHG0gBHt8EXxS6T6tJRTGuDQVnluuAvMxWF4j8SNFDKceLFg=="
	defer></script>
```