---
pageTitle: Sub-apps
blurb: "A guide to creating sub-apps in Sienar"
pageNumber: 4
tags:
  - guides
---

# Sub-apps

Sienar is designed to allow different plugins to execute at different endpoints if they choose. Used in conjunction with the `IPLuginExecutionTracker`, developers can create sub-apps that execute exclusively at specific endpoints.

For Sienar's purposes, consider a sub-app any plugin that executes at an endpoint while excluding other plugins from executing at that endpoint. If a plugin executes on `/dashboard` routes, it is a sub-app **if and only if** it prevents other plugins from also executing on `/dashboard`.

## A simple sub-app

### Limiting a plugin to a specific route

The `SienarCmsPlugin` only executes on the `/dashboard` endpoint. A straightforward way to achieve this is to check if the current request starts with `/dashboard`:

```csharp
using Microsoft.AspNetCore.Http;
using Sienar.Infrastructure.Plugins;

namespace Example;

public class CustomPlugin : ISienarPlugin
{
	private readonly IHttpContextAccessor _contextAccessor;

	public CustomPlugin(IHttpContextAccessor contextAccessor)
	{
		_contextAccessor = contextAccessor;
	}

	/// <inheritdoc />
	public bool ShouldExecute()
		=> _contextAccessor.HttpContext!.Request.Path.StartsWithSegments("/dashboard");
}
```

As you can see, any request starting with `/dashboard` will cause this plugin to execute. This may be all you need for your plugin. For example, if you want to add pages to the `/dashboard` for users or administrators to interact with your plugin, this is the easiest way to ensure that your plugin executes *alongside* other plugins that run on `/dashboard`. But just executing on a specific route doesn't make a plugin a sub-app.

### Creating a sub-app

The previous example isn't a sub-app in the Sienar sense because it doesn't disallow other sub-apps from executing. In order to do that, it has to use the `IPluginExecutionTracker`.

```csharp
using Microsoft.AspNetCore.Http;
using Sienar.Infrastructure.Plugins;

namespace Example;

public class CustomPlugin : ISienarPlugin
{
	private readonly IHttpContextAccessor _contextAccessor;
	private readonly IPluginExecutionTracker _executionTracker;

	public CustomPlugin(
		IHttpContextAccessor contextAccessor,
		IPluginExecutionTracker executionTracker)
	{
		_contextAccessor = contextAccessor;
		_executionTracker = executionTracker;
	}

	/// <inheritdoc />
	public bool ShouldExecute()
	{
		if (_contextAccessor.HttpContext!.Request.Path.StartsWithSegments("/dashboard"))
		{
			_executionTracker.ClaimExecution();
			return true;
		}

		return false;
	}
}
```

By adding a call to `IPluginExecutionTracker.ClaimExecution()`, your plugin informs other plugins that it has executed (or "claimed execution"), so they should not execute even if they otherwise might wish to. This is *almost* a sub-app, and technically, it works as a sub-app in the sense that it prevents other sub-apps from executing. However, your sub-app will still execute even if another sub-app has already executed. To change this, you need to first check the `IPluginExecutionTracker.SubAppHasExecuted` property:

```csharp
using Microsoft.AspNetCore.Http;
using Sienar.Infrastructure.Plugins;

namespace Example;

public class CustomPlugin : ISienarPlugin
{
	private readonly IHttpContextAccessor _contextAccessor;
	private readonly IPluginExecutionTracker _executionTracker;

	public CustomPlugin(
		IHttpContextAccessor contextAccessor,
		IPluginExecutionTracker executionTracker)
	{
		_contextAccessor = contextAccessor;
		_executionTracker = executionTracker;
	}

	/// <inheritdoc />
	public bool ShouldExecute()
	{
		if (_executionTracker.SubAppHasExecuted) return false;
		if (_contextAccessor.HttpContext!.Request.Path.StartsWithSegments("/dashboard"))
		{
			_executionTracker.ClaimExecution();
			return true;
		}

		return false;
	}
}
```

Now, your plugin is executing like a sub-app. Because you first check if any other sub-app has executed, you won't step on any other plugin's toes, and because you let other sub-apps know that your sub-app is executing, other plugins know to stay out of your way.

The reason you should check if another sub-app has executed is in case another sub-app wants to use the same route as you. For example, the `SienarCmsPlugin` uses `/dashboard` as its base route, but your plugin might want to use `/dashboard/<your-plugin-name>` as its base route. As long as your plugin is registered before `SienarCmsPlugin`, this will work as expected, and in general, your plugin shouldn't care what another plugin uses as its base route.

Because this logic represents a common pattern, it has been implemented as an extension method, `IPluginExecutionTracker.ExecuteAsSubApp()`, which accepts the `HttpContext` as its first argument and the sub-app route string as its second argument. You can refactor your plugin to use this extension method:

```csharp
using Microsoft.AspNetCore.Http;
using Sienar.Extensions;
using Sienar.Infrastructure.Plugins;

namespace Example;

public class CustomPlugin : ISienarPlugin
{
	private readonly IHttpContextAccessor _contextAccessor;
	private readonly IPluginExecutionTracker _executionTracker;

	public CustomPlugin(
		IHttpContextAccessor contextAccessor,
		IPluginExecutionTracker executionTracker)
	{
		_contextAccessor = contextAccessor;
		_executionTracker = executionTracker;
	}

	/// <inheritdoc />
	public bool ShouldExecute()
		=> _executionTracker.ExecuteAsSubApp(
			_contextAccessor.HttpContext!,
			"/dashboard");
}
```

## Limitations

As you can see, this system is very useful for limiting your app to executing only when other sub-apps haven't already executed. However, the biggest limitation of this system is that it isn't enforceable.

The sub-app examples above show that in order for a sub-app to skip execution if another sub-app has already executed, it has to manually exclude *itself*. There's no true way to prevent another plugin from executing if it wants to, which is by design. It's perfectly valid for multiple plugins to execute at a specific route. For example, you might want to add your plugin's configuration and administration pages to the `/dashboard` app. This is fine to do, so the `SienarCmsPlugin` can't prevent your plugin from executing on the `/dashboard` route. The system is unenforceable and opt-in, which provides flexibility to developers to create systems that they want to create.