# Sienar
## An application development framework built on top of Blazor Server and MudBlazor

Sienar is a framework for building desktop and web applications. When building web applications with Sienar, a small CMS is also included.

## Philosophy

Sienar is designed to allow the developer to build hookable and modular applications. The basic design philosophy was loosely inspired by WordPress' hooks and plugin systems; Sienar offers similar functionality via a variety of interfaces.

### Sienar Web

Sienar is built on top of Blazor Server and the MudBlazor component library. These are opinionated decisions intended to speed up the initial application development process by avoiding [decision paralysis](https://en.wikipedia.org/wiki/Analysis_paralysis). While Blazor Server and MudBlazor both have drawbacks, neither option is objectively worse when weighed against the alternatives. While Blazor Server requires additional server resources, it also greatly simplifies application architecture and overall decreases the time needed to develop features; Blazor Server apps also don't require more resources on the low end, so by the time performance becomes a problem, an application should have the financial backing to scale out or refactor. And while MudBlazor may lack robust customization options, the default components are attractive, performant, functional, and - most importantly - free. MudBlazor components are only forced in the default layout and dashboard, so if you want to use different components for other parts of your app, you can. You also don't have to use the Sienar-provided UI at all if you don't want to (more on that later).

### Sienar Desktop

When developing for desktop, Sienar utilizes [Electron.NET](https://github.com/ElectronNET/Electron.NET). This is, again, an opinionated decision merely meant to force an option among a wealth of .NET desktop development options, few of which are as cross-platform as Electron. I was hesitant to join in the over-utilization of Electron in desktop development, but I'm far more hesitant about the long-term viability of .NET MAUI given Microsoft's poor track record with UI frameworks. Since no other UI framework supports Blazor as a first-class citizen, I decided that Electron.NET was the best decision for Sienar, even though I try to avoid Electron as a general rule.

## Basic application design

Sienar is designed to make creating pluggable apps easier. For that reason, the boilerplate for a Sienar app looks quite a bit different from a traditional ASP.NET app. The standard boilerplate looks about like this:

```csharp
// Program.cs

await SienarWebAppBuilder
	.Create<AppDbContext>(
		args,
		o => o.UseSienarDb(),
		ServiceLifetime.Transient)
	.AddPlugin(new SienarBlazorPlugin())
	.AddPlugin(new SienarCmsPlugin())
	.AddPlugin(new MailKitPlugin())
	.ConfigureTheme(new CustomTheme())
	.Build()
	.RunAsync();
```

This boilerplate shows how to initialize a Sienar app, complete with login, account management, user moderation, SMTP email, and more.

The `SienarBlazorPlugin` adds core Sienar functionality to the application. With it, you can use data readers and writers to manage Sienar entities from code without a UI or other out-of-the-box functionality like login. You can set up code to use Sienar-provided classes like `ISignInManager` to implement your own log in rules if you want. Use the `SienarBlazorPlugin` alone if you want to create your own UI.

The `SienarCmsPlugin` adds the out-of-the-box functionality you would expect from a CMS. By adding this plugin, you get a full UI to manage your own user account, moderate other users, and other functionality like logging in, password reset, and more.

The `MailKitPlugin` adds SMTP mailing capabilities to your app. By default, Sienar doesn't know how to send emails to users, so you can either disable email entirely in `appsettings.json`, or you can enable mailing by using `MailKitPlugin`. You can also create your own mailing plugin using the guide at the end of this README, under `Customization > Creating your own mail sending plugin`

### `SienarWebAppBuilder.Create()`

This static method creates a new `SienarWebAppBuilder`. This uses a similar pattern to ASP.NET with the `WebApplication.CreateBuilder()` method (in fact, Sienar uses this internally). There are two overloads to the `Create()` method: one that accepts the `string[] args` argument to `Program.Main()`, and one that accepts additional arguments that configure a `DbContext`. The second `Create()` overload accepts the same arguments as `IServiceCollection.AddDbContext<TContext>()` after the `string[] args` argument (because, again, Sienar uses this internally to register your DbContext). Sienar itself avoids a lot of generic code by requesting the base type `DbContext` from the DI container, so one thing that `Create<TContext>()` does is re-registers your `TContext` as a `DbContext` so it can be used directly by Sienar without needing to know the implementation type.

The example code creates an app with a `CustomDbContext` that has a transient lifetime. This is recommended for Sienar applications that use the Blazor UI because a scoped `DbContext` is too long a lifetime in Blazor Server. Microsoft has [several workarounds](https://learn.microsoft.com/en-us/aspnet/core/blazor/blazor-ef-core?view=aspnetcore-8.0) for the service lifetime problem, but I find these to be too shallow for Sienar's needs. Internally, Sienar uses a *scoped* service to access the current `DbContext`, but refreshes the `DbContext` on page navigations by using a base page class - so even though the `DbContext` itself is transient, it is accessed via a scoped service that keeps it around for the proper duration (one page visit = one `DbContext`). This system is similar to how Microsoft solved Blazor scope issues with `OwningComponentBase`, but that class requires direct usage of `IServiceProvider`, or else limits you to a single component-scoped service.

### `SienarWebAppBuilder.AddPlugin()`

This extension method tells Sienar that you want to use a plugin. Your plugin is registered as a singleton in the DI container, and Sienar needs an instance of your plugin from step 1, so instead of messing with intermediate DI containers or requiring default constructors for plugins, Sienar just asks for an instance. It's up to you to provide that instance.

Plugins must implement the `ISienarPlugin` interface, which is discussed later. For now, suffice it to say that plugins have the ability to register services in the DI container, configure the `IApplicationBuilder` with middleware, and hook directly into core Sienar functionality, like adding menu items to the account dashboard or overriding certain components in the UI.

### `SienarWebAppBuilder.ConfigureTheme()`

This extension method allows you to override the default `MudTheme` used by MudBlazor - if you don't care about the theme, or you're just prototyping for now, you can skip this method call and the default `MudTheme` will be used. You can also pass an optional `bool isDarkMode` as the second argument, which tells MudBlazor that your app should use dark mode. Both of these values end up stored in a scoped state provider, `ThemeState`.

### `SienarWebAppBuilder.AddServices()`

This extension method (not shown here) allows you to add arbitrary services to the `IServiceCollection`. This can be useful for overriding Sienar services (most of which are added to DI conditionally with `IServiceCollection.TryAdd____()`) or else just adding a few items to DI without creating a full plugin. It accepts a single argument, an `Action<IServiceCollection>`.

### `SienarWebAppBuilder.Build()`

This method performs the final step of the Sienar building process. It performs internal tasks such as setting up the MudBlazor theme and configuring plugins and middleware. The return result is a regular ASP.NET `WebApplication`, so if you want to do any final tasks outside the Sienar scope like adding final middleware, you can do that before calling `WebApplication.RunAsync()`

## Plugin system

Sienar offers plugin-like functionality via the `Sienar.Infrastructure.Plugins.ISienarPlugin` interface. This interface defines a number of methods and properties that Sienar uses to provide your plugin's behavior to a Sienar app.

### `ISienarPlugin.PluginData`

This property contains an instance of the [PluginData](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.BlazorUtils/Infrastructure/Plugins/PluginData.cs) class, which provides users with information about the plugin, such as the name of the plugin, the name of the author, the plugin's website, and more.

### `ISienarPlugin.PluginSettings`

This property contains an instance of the [PluginSettings](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.BlazorUtils/Infrastructure/Plugins/PluginSettings.cs) class, which tells Sienar which basic features the plugin uses. Currently, `PluginSettings` has two properties: `bool HasRoutableComponents`, which tells Sienar to include your plugin's assembly in the `<Router>` component, and `bool UsesProviders`, which tells Sienar that your plugin uses one or more configuration providers, which support features that get configured on a per-session basis (more on that in a moment).

### `ISienarPlugin.SetupDependencies()`

This method is used by plugins to do any initial setup, such as adding services to the DI container. This method accepts the ASP.NET `WebApplicationBuilder` as its only argument, so if you can do it in plain ASP.NET, you can do it here. Don't build the builder though! Sienar handles the build process, so just add your plugin's configuration here and let Sienar do the rest.

### `ISienarPlugin.SetupApp()`

This method is used by plugins to configure the middleware pipeline. Most plugins probably won't have to do anything here, so a stub implementation should do fine. If your plugin defines a URL slug there a whole new application should be served, you should call `IEndpointRouteBuilder.MapFallbackToPage()` here. For example, `SienarCmsPlugin` only serves its pages from `/dashboard`, so it calls `app.MapFallbackToPage("/dashboard/{**segment}", "/_Host")`. You can either set up your own fallback page in your app, or if you want to rely on the Sienar system, you can fall back to `/_Host`, which comes with Sienar and includes all the regular Sienar page-level functionality right out of the box.

### `ISienarPlugin.PluginShouldExecute()`

This method is used by Sienar middleware to determine if the plugin should execute on each request. (Because Blazor Server treats each "request" as an entire user session, this method will only run on each full page load.) It accepts an `HttpContext` as its only argument, so you can use that context to decide whether the application should execute. For example, `SienarBlazorPlugin` does work that is needed on every request to a Sienar application, so its implementation simply returns `true`, while `SienarCmsPlugin` should only execute if the user is trying to access the dashboard, so it returns `context.Request.Path.StartsWithSegments("/dashboard")` to only perform plugin actions on dashboard pages.

### `ISienarPlugin.SetupMenu()`

This method is the first method that uses a configuration provider - namely, the `IMenuProvider`, which this method accepts as its only argument. This configuration provider allows your plugin to add items to various menus of different pages in an app. More information is available in the guide at the end of this README, under `Customization > Using configuration providers > IMenuProvider`.

### `ISienarPlugin.SetupDashboard()`

This method also uses the `IMenuProvider`, but in a different way than `SetupMenu()`. The scoped DI container actually contains two implementations of `IMenuProvider`, which are keyed to the `SetupMenu()` and `SetupDashboard()` purposes. While `SetupMenu()` supports adding menu items to page-level menus, `SetupDashboard()` supports adding links to a series of sections, similar to the cPanel UI. Dashboards support a limited set of features compared to menus. More information is available in the guide at the end of this README, under `Customization > Using configuration providers > IMenuProvider`.

### `ISienarPlugin.SetupStyles()`

This method uses the `IStyleProvider`, which allows plugins to enqueue CSS files for inclusion on the webpage. For example, the `SienarBlazorPlugin` uses the `IStyleProvider` to load MudBlazor's CSS on every page of a Sienar app. The `IStyleProvider` has support for HTML features of including CSS files, including things like the `crossorigin` and `integrity` attributes. Simply omit any value you don't need, and it won't appear in the HTML. More information is available in the guide at the end of this README, under `Customization > Using configuration providers > IStyleProvider`.

### `ISienarPlugin.SetupScripts()`

This method uses the `IScriptProvider`, which works similarly to the `IStyleProvider` to provide JavaScript inclusion on webpages. The `IScriptProvider` supports most scenarios for including JavaScript on a webpage, including whether the script should be `async`, whether the script should `defer`, whether the script is a module or a regular JavaScript file, etc. Simply omit any value you don't need, and it won't appear in the HTML. More information is available in the guide at the end of this README, under `Customization > Using configuration providers > IScriptProvider`.

### `ISienarPlugin.SetupComponents()`

This method uses the `IComponentProvider`, which is a container that includes references to the `Type`s of components that should be used in various parts of the Sienar UI. More information is available in the guide at the end of this README, under `Customization > Using configuration providers > IComponentProvider`.

## Hooks system

Sienar includes a system of hooks similar to that of WordPress. However, instead of hooking into actions with string names, Sienar allows you to hook into actions with strongly-typed interfaces. Each type of hookable action supports a specific group of interfaces, so in order to hook into a specific action, you need to implement the correct interface with the correct generic model type.

### Basics: Actions, requests, processors, hooks, and services

Sienar uses five pieces of terminology in regards to its hook system.

**Actions** are things you want to do in your code. An example of an action would be logging in to the app. There are seven general types of actions: `Read`, `ReadAll`, `Create`, `Update`, `Delete`, `Action`, and `ResultAction`. These action types are stored in the [ActionType enum](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.BlazorUtils/Infrastructure/Hooks/ActionType.cs), and they are used by hooks to determine whether a hook should run for a particular request. Most of these action types refer to CRUD-based actions, but the `Action` and `ResultAction` types are very generalized. An `Action` is an action that just has to run, and it returns a `bool` indicating whether it succeeded. A `ResultAction` is an action that returns a result, such as a file, or `null` on failure.

**Requests** are classes that represent an **action**. Usually, these classes contain data, but they don't have to. For example, the [LoginRequest](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.Cms/Identity/Requests/LoginRequest.cs) requires the user's username and password, along with an optional property indicating whether the login should persist. While the `LoginRequest` class contains data, the [LogoutRequest](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.Cms/Identity/Requests/LogoutRequest.cs) class does not - it merely logs out the current user, which the app determines through other means. But even though the `LogoutRequest` doesn't have any data, the empty class is still used to strongly type the logout process in the hook system.

**Processors** are classes that use **requests** to perform an **action**. Each action corresponds to exactly one processor. For example, logging in to the Sienar dashboard uses the [LoginProcessor](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.Cms/Identity/Processors/LoginProcessor.cs), which itself relies on the `LoginRequest` we discussed in the previous paragraph.

**Hooks** are classes that run before or after **processors**. Some hooks give you the ability to short-circuit an **action**, while others only allow you to respond to the result of an **action** after it has already happened.

**Services** are classes that encapsulate all this behavior, from beginning to end, using hooks and processors. Most Sienar services perform a single **action**, although there are a couple exceptions. A service uses the type of the **request** (e.g., `LoginRequest`) to get a **processor** from the DI container (e.g., `LoginProcessor`). The service also requests **hooks** associated with the type of the **request**, if any (hooks are requested as `IEnumerable<THook>`, so you can have as many hooks as you want, or none at all).

**NOTE**: unless you want to change the way a service works in general, you don't need to create a service class. Sienar comes with its own service class, which is fully generic and performs this pipeline for every kind of request, including custom requests you create. In order to hook into an existing Sienar **action**, you only need to implement the appropriate **hook** types, and in order to create a fully custom process, you only need to create the **request** type and the **processor** type. You can even create **hooks** for your custom actions.

### The four hooks

There are three main types of services that come built into Sienar: action services (`ActionType.Action`), result services (`ActionType.ResultAction`), and CRUD services (which use the remaining `ActionType`s). All three of these services use hooks in slightly different ways, but they all use the same four core hooks.

Some hooks return a [HookStatus](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.BlazorUtils/Infrastructure/Hooks/HookStatus.cs), which is a general indicator of whether the hook succeeded or failed and why. If a hook returns a `HookStatus`, it can be used to short-circuit an action. If a hook indicates failure, this won't stop other hooks of the same type from running, but it *will* prevent the process from moving on to the next step.

#### `IBeforeProcess<TEntity>`

The `IBeforeProcess<TRequest>` hook executes before the `IProcessor<TRequest>`. It receives the `TRequest` and the `ActionType` as its arguments, and it returns a `HookStatus`, so it can short-circuit a request.

The `IBeforeProcess<TRequest>` hook can be used to run code on each action. For example, the [ConcurrencyStampUpdateHook](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.BlazorUtils/Infrastructure/Hooks/ConcurrencyStampUpdateHook.cs) refreshes a database entity's concurrency stamp prior to creating or updating the entity.

The `IBeforeProcess<TRequest>` can also be used to verify certain 

#### CRUD services

There are actually three types of CRUD service: `IEntityReader`, which gets individual or multiple database entities; `IEntityWriter`, which creates and updates individual database entities; and `IEntityDeleter`, which deletes individual database entities. Each of these services has its own hooks.

##### `IBeforeRead<TEntity>`

The `IBeforeRead<TEntity>` hook is used to make modifications to the `Filter` that is used to read entities from the database. Because the `Filter` is nullable, the `IBeforeRead<TEntity>` hook returns a `Filter?`, which allows modifications to a `Filter` that is null. Because `IBeforeRead<TEntity>` returns a `Filter?`, it cannot be used to short-circuit read requests.

##### `IAfterRead<TEntity>`

The `IAfterRead<TEntity>` hook executes after an entity has been read from the database. It receives the entity as its first argument, and a `bool isSingle` as its second argument, which indicates whether the process is reading a single entity or reading all entities. You can make modifications to the `TEntity` at this point if you wish, but these modifications are not guaranteed to be persisted.

The `IAfterRead<TEntity>` hook returns a `HookStatus`, so it can short-circuit the read process. An example of this is the [VerifyUserCanReadFileHook](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.Cms/Media/Hooks/VerifyUserCanReadFileHook.cs), which runs after fetching an `Upload` from the database. If the user doesn't actually have permission to access the file (which can't be determined before the file is read), this hook returns `HookStatus.Unauthorized`, which will cause the `IEntityReader<Upload>` to return either `null` or an empty `PagedQuery<Upload>`.

##### `IBeforeUpsert<TEntity>`

The `IBeforeUpsert<TEntity>` hook executes before adding or updating an entity. It receives the entity as its first argument and a `bool isAdding` as its second argument, which indicates whether the process is creating a new entity or updating an existing entity. Any modifications you make to entities at this point will be persisted if no other hooks short-circuit the process.

`IBeforeUpsert<TEntity>` can be used to short-circuit the process because it returns a `HookStatus`. An example of this is the [VerifyUserCanModifyFileHook](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.Cms/Media/Hooks/VerifyUserCanModifyFileHook.cs), which runs both before upsert and before delete to make sure that a user has permission to modify the upload in question and short-circuits if they don't.

`IBeforeUpsert<TEntity>` can also be used for non-verification purposes. For example, the [ConcurrencyStampUpdateHook](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.BlazorUtils/Infrastructure/Hooks/ConcurrencyStampUpdateHook.cs) is used to update the concurrency stamp of entities before upsert.

##### `IAfterUpsert<TEntity>`

The `IAfterUpsert<TEntity>` hook executes after adding or updating an entity. It receives the entity as its first argument and a `bool isAdding` as its second argument, which indicates whether the process is creating a new entity or updating an existing entity.

`IAfterUpsert<TEntity>` can't be used to short-circuit the upsert process because the upsert has already taken place. Use `IAfterUpsert<TEntity>` to respond to events. For example, `IAfterUpsert<SienarUser>` could be used to email a user whose account was created in the UI by an administrator. (Although no such implementation exists, it is technically feasible to do so.)

##### `IBeforeDelete<TEntity>`

The `IBeforeDelete<TEntity>` hook executes before deleting an entity. It receives the entity as its only argument and returns a `HookStatus`, so it can be used to short-circuit the deletion process. The `VerifyUserCanModifyFileHook` we saw before is used to implement both `IBeforeDelete<Upload>` and `IBeforeUpsert<Upload>` because the logic is the same whether a user is trying to modify or delete their own file.

##### `IAfterDelete<TEntity>`

The `IAfterDelete<TEntity>` hook executes after deleting an entity. It receives the entity as its only argument. It can't be used to short-circuit the deletion process because it doesn't return anything. For example, the [ForceDeletedAccountLogoutHook](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.Cms/Identity/Hooks/ForceDeletedAccountLogoutHook.cs) forces a user session to end immediately when a user's account is deleted.

## Customizations

### Using configuration providers

Sienar has several configuration providers that set up various aspects of a Sienar app at the start of each user session (i.e., page load). One of the main advantages of the Sienar plugin system is that it allows apps to hook into these configuration providers without manually interacting with DI themselves.

#### `IMenuProvider`

### Creating your own mail sending plugin

By default, Sienar doesn't know how to send email. I wrote the `MailKitPlugin`, which is a wrapper around [MailKit](https://github.com/jstedfast/MailKit), a popular .NET library that simplifies sending email from C#. This plugin requires that you have SMTP access to a mail server. However, you might want to send email via a REST API instead (such as SendGrid or SparkPost), or else use a different implementation of SMTP. If you want to do that, you need to create your own wrapper around the service you want to use. In Sienar, this is pretty straightforward - you can do this as a full plugin if you want to distribute your solution to others, or you can use the `SienarWebAppBuilder.AddServices()` extension method if you just want to add the necessary configuration for your own apps.

To create your own mailer, you must create a class that implements the `Sienar.Email.IEmailSender` interface. After that, all you need to do is place that class in dependency injection as a transient service. If you want to keep the Sienar default implementation out of DI altogether, either add your implementation before adding the `SienarBlazorPlugin` or remove the Sienar implementation using the `IServiceCollection.RemoveService()` extension method, which is provided in the `Sienar.BlazorUtils` project.