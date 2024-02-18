---
pageTitle: Hooks
blurb: "An overview of the Sienar hook system"
pageNumber: 4
tags:
  - introduction
---

# Title

Sienar includes a system of hooks similar to that of WordPress. However, instead of hooking into actions with magic strings, Sienar allows you to hook into actions with strongly-typed interfaces. Each type of hookable action supports a specific group of interfaces, so in order to hook into a specific action, you need to implement the correct interface with the correct generic model type.

## Basics: Actions, requests, processors, hooks, and services

Sienar uses five pieces of terminology in regards to its hook system.

**Actions** are things you want to do in your code. An example of an action would be logging in to the app. There are seven general types of actions: `Read`, `ReadAll`, `Create`, `Update`, `Delete`, `Action`, and `ResultAction`. These action types are stored in the [ActionType enum](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.BlazorUtils/Infrastructure/Hooks/ActionType.cs), and they are used by hooks to determine whether a hook should run for a particular request. Most of these action types refer to CRUD-based actions, but the `Action` and `ResultAction` types are very generalized. An `Action` is an action that just has to run, and it returns a `bool` indicating whether it succeeded. A `ResultAction` is an action that returns a result, such as a file, or `null` on failure.

**Requests** are POCOs that represent an **action**. Usually, these classes contain data, but they don't have to. For example, the [LoginRequest](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.Cms/Identity/Requests/LoginRequest.cs) requires the user's username and password, along with an optional property indicating whether the login should persist. While the `LoginRequest` class contains data, the [LogoutRequest](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.Cms/Identity/Requests/LogoutRequest.cs) class does not - it merely logs out the current user, which the app determines through other means. But even though the `LogoutRequest` doesn't have any data, the empty class is still used to strongly type the logout process in the hook system.

**Processors** are methods that use **requests** to perform an **action**. Each action corresponds to exactly one processor. For example, logging in to the Sienar dashboard uses the [LoginProcessor](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.Cms/Identity/Processors/LoginProcessor.cs), which itself relies on the `LoginRequest` we discussed in the previous paragraph.

**Hooks** are methods that run before or after **processors**. Some hooks give you the ability to short-circuit an **action**, while others only allow you to respond to data.

**Services** are classes that encapsulate all this behavior, from beginning to end, using hooks and processors. Most Sienar services perform a single **action**, although there are a couple exceptions. A service uses the type of the **request** (e.g., `LoginRequest`) to get a **processor** from the DI container (e.g., `LoginProcessor`). The service also requests **hooks** associated with the type of the **request**, if any (hooks are requested as `IEnumerable<THook>`, so you can have as many hooks as you want, or none at all).

## The three service types

There are three main types of services that come built into Sienar: action services (`ActionType.Action`), result services (`ActionType.ResultAction`), and CRUD services (which use the remaining `ActionType` values). All three of these services use hooks in slightly different ways, but they all use the same core hooks.

### Action services

An action service wraps around an `IProcessor<T>`, which has a handful of methods. `NotifySuccess()`, `NotifyFailure()`, and `NotifyNoPermission()` methods allow the processor to inform the user of various statuses, while the `Process()` method accepts a `T` and returns a `HookStatus`. You can request an action service by requesting an `IService<T>` from DI. The only thing you absolutely need in order to crease your own action service is to implement `IProcessor<T>` and add it to the DI container (probably as a transient), where `T` is the data type that your action needs. Sienar provides the `IService<T>` implementation - all you need is your `T` and your `IProcessor<T>`.

The previously-mentioned `LoginProcessor` is used to perform a login operation. In order to actually perform that login operation, you need to request an `IService<LoginRequest>` from DI and call its `Execute()` method, which expects a single argument of type `LoginRequest` and returns a `Task<bool>` indicating the success status of the action. Sienar does the heavy lifting, like executing hooks, performing error handling, and notifying the user of any problems (or successes). You can see `IService<LoginRequest>` in action on the [Sienar login page](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.Cms/Pages/Account/Login.razor).

### Result services

A result service wraps around an `IResultProcessor<T>`, which has the same methods as `IProcessor<T>`. The only difference is that `IResultProcessor<T>.Process()` has no arguments and returns a tuple of `(HookStatus, T?)`. Result services are used to perform actions that don't require input but do produce output. You can request a result service by requesting an `IResultService<T>` from DI. As with `IService<T>`, the only thing you must provide is an `IResultProcessor<T>` in the DI container, where `T` is the data type you expect to return. After that, you can request an `IResultService<T>` and use its `Execute()` method, which accepts no arguments and returns a `Task<T?>`.

An example of an `IResultProcessor<T>` is the [PersonalDataProcessor](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.Cms/Identity/Processors/PersonalDataProcessor.cs), which is called by Sienar when the user asks to download their personal data on the [personal data page](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.Cms/Pages/Account/PersonalData.razor).

### CRUD services

CRUD services are a little different than action services and result services because they don't really use processors, per se. CRUD services perform CRUD operations on database entities directly, so instead of using general processors, they directly implement the functionality they need.

`IEntityReader<T>` is used to read entities from the database. It has two overloads both named `Read()`: one which requires a `Guid` and accepts an optional `Filter?`, and one which only accepts an optional `Filter?`. The overload which requires a `Guid` returns a single `T?`, while the other overload returns a `PagedQuery<T>`. The `Filter` can be used to do things like page through results and include relations in the EntityFramework query.

`IEntityWriter<T>` is used to write entities to the database. It has two methods: `Create()`, which accepts a `T` as its only argument and returns a `Guid` representing the ID of the entity in the database; and `Update()`, which accepts a `T` as its only argument and returns a `bool` representing whether the update operation was successful.

`IEntityDeleter<T>` is used to delete entities from the database. It has a single method: `Delete()`, which accepts a `Guid` as its only argument and returns a `bool` representing whether the delete operation was successful.

CRUD services are grouped together largely for semantic purposes. `IEntityReader<T>` has two methods that use the same hooks in the same way, and the same is true of `IEntityWriter<T>`. `IEntityDeleter<T>` doesn't use its hooks the same way as either of the other two services, so it exists on its own.

## The four hooks

Now that we understand what services are and what they do, we can dig into the four hooks that Sienar provides for modifying the behavior of services.

### IAccessValidator<T>

The `IAccessValidator<T>` hook determines whether access should be granted to a particular action. Similar to data annotations, `IAccessValidator<T>` relies on a validation context in order to grant access - in other words, if no `IAccessValidator<T>` explicitly grants access on a request, access will be **denied** by default. The only exception to this rule is if there are no `IAccessValidator<T>` hooks at all for a request, in which case access will still be granted. The `IAccessValidator<T>` hooks are run before any other hook, so they can be used to short-circuit an action by failing to grant explicit access.

An example of an `IAccessValidator<T>` is the [UserInRoleAccessValidator](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.BlazorUtils/Infrastructure/Hooks/UserInRoleAccessValidator.cs), which verifies that a user is in a particular role before approving access.

### IStateValidator<T>

The `IStateValidator<T>` hook is used to determine whether the state of `T` is valid compared to the state of the application. This can be as simple as verifying that a property has the correct value or as complex as querying the database to check deep application state. `IStateValidator<T>` returns a `HookStatus`, so it can be used to short-circuit an action by returning a failing value.

An example of a simple `IStateValidator<T>` is the [RegistrationOpenValidator](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.Cms/Identity/Hooks/RegistrationOpenValidator.cs), which verifies that the site administrator has activated registration in `appsettings.json` before allowing a user to register.

An example of a more complex `IStateValidator<T>` is the [EnsureAccountInfoUniqueValidator](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.Cms/Identity/Hooks/EnsureAccountInfoUniqueValidator.cs), which checks the database for an existing entity using a particular username or email address before creating a user account. `EnsureAccountInfoUniqueValidator` is also an example of a single class being used for two different hooks, which is unusual but occasionally appropriate. In this case, the hook runs before a regular user tries to register *and* before an administrator tries to create or modify a user account in the admin UI. Because the logic is indentical in either case, the hook uses a core `UserIsUnique()` method that is called from both hook methods.

### IBeforeProcess<T>

The `IBeforeProcess<T>` hook runs immediately before the action processor. It's used to do things like modify the state of `T` prior to processing. This hook only returns a `Task`, so it can't directly be used to short-circuit an action. However, if an exception is thrown while an `IBeforeProcess<T>` is executing, it will still short-circuit because each `IBeforeProcess<T>` is guaranteed to run before any non-read action - so if an `IBeforeProcess<T>` fails to execute, the action will end early.

An example of an `IBeforeProcess<T>` is the [ConcurrencyStampUpdateHook](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.BlazorUtils/Infrastructure/Hooks/ConcurrencyStampUpdateHook.cs), which refreshes an entity's concurrency stamp on create or update actions.

### IAfterProcess<T>

The `IAfterProcess<T>` hook runs after the action processor. It's used to do things in response to the result of an action. This hook only returns a `Task`, so it can't be used to prevent the rest of the action from continuing. Unlike other hook types, if an `IAfterProcess<T>` throws an exception, other `IAfterProcess<T>` hooks will still be executed and the result of the operation will still be returned. If an `IAfterProcess<T>` fails, it's up to the developer to notify the user of any consequences of failure.

An example of an `IAfterProcess<T>` is the [ForceDeletedAccountLogoutHook](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.Cms/Identity/Hooks/ForceDeletedAccountLogoutHook.cs), which checks for an active user session when an account is deleted, and if one is found, ends the user session immediately.

## How services use hooks

As alluded before, different types of services use hooks in slightly different ways. Here, we'll discuss how each of the services use hooks.

### Action service

The action service is the prototypical service in the documentation, so anytime the docs discuss how services work *in general*, they're probably talking about action services. That said, action services operate roughly as described right here in the docs when discussing hooks.

1. All `IAccessValidator<T>` hooks are executed
2. `IStateValidator<T>` hooks are executed
3. All `IBeforeProcess<T>` hooks are executed
4. `IProcessor<T>` is executed. If it throws or indicates failure, the action service returns `false`
5. `IAfterProcess<T>` hooks are executed

### Result service

Because the result service is intended to generate output instead of accept input, it uses hooks differently than the general action service.

1. All `IAccessValidator<T>` hooks are executed, with `default(T)` passed as the value to the `input` argument (since no action has been taken, `T` doesn't exist yet. Access validation needs to define access rules that don't depend on `T`)
2. `IResultProcessor<T>` is executed. If it a) throws, b) indicates failure, or c) returns `null`, the result action service returns `default(T)`
3. `IAfterProcess<T>` hooks are executed with the result of the process, which is guaranteed not to be `null`

Two hooks don't run with result services at all: The `IStateValidator<T>` doesn't execute because there's no input state to validate, and `IBeforeProcess<T>` doesn't execute because there's no input to act against.

### CRUD services

Each CRUD service also uses hooks differently, both from other service types and from each other CRUD service. We'll treat each CRUD service separately.

CRUD services don't use processors as such. Instead, CRUD services operate on entities directly in the database using Entity Framework.

#### Entity reader service

Much like a result service, the entity reader service generates output. However, the entity reader service accepts a [Filter?](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.BlazorUtils/Infrastructure/Entities/Filter.cs) as input, which provides filtering, sorting, and related entity loading data to the entity reader.

Filters are converted into Entity Framework LINQ method calls by an [IFilterProcessor<TEntity>](https://github.com/christianlevesque/sienar/blob/main/src/Sienar.BlazorUtils/Infrastructure/Processors/IFilterProcessor.cs), which is used only by the entity reader service. This `IFilterProcessor<TEntity>` is not considered a hook because only a single instance is requested. While it's not a hook, it can still be overridden on a per-entity basis because each entity registers its `IFilterProcessor<TEntity>` with `IServiceCollection.TryAddTransient()`.

For single read operations (`Read(Guid, Filter?)`):

1. The entity is fetched from the database. If it was not found, the read action returns `null`
2. All `IAccessValidator<TEntity>` hooks are executed with the `TEntity` passed as the `input` argument
3. All `IAfterProcess<TEntity>` hooks are executed

For multiple read operations (`Read(Filter?)`):

1. The entities are fetched from the database and buffered into memory (for large entity sets, you should be paging results using the `Filter`)
2. `IAfterProcess<TEntity>` hooks are executed

Because the entity is being read, no state changes will take place, so `IStateValidator<TEntity>` and `IBeforeProcess<TEntity>` are not used.

Furthermore, on multiple-read actions, `IAccessValidator<TEntity>` is not executed either. This is a design decision that was made to avoid the complexities of paging through results while accounting for entities that a user doesn't have permission to view. Instead of using an access validator to validate access permission after the fact, developers should opt to use the `IFilterProcessor<TEntity>` to ensure that unallowed results aren't returned in the first place (for example, add a call to `Where(entity => entity.UserId == <user-id>)` to ensure a user only receives their own results).

#### Entity writer service

The entity writer service methods both accept a `TEntity` as input. However, the `Create(TEntity)` method returns a `Guid` representing the ID of the newly-created entity, while the `Update(TEntity)` method returns a `bool` representing the success of the update. Both methods follow the standard order of operations for action services.

1. All `IAccessValidator<TEntity>` hooks are executed
2. All `IStateValidator<TEntity>` hooks are executed
3. All `IBeforeProcess<TEntity>` hooks are executed
4. The `TEntity` is either inserted or updated in the database. If this process throws, the entity writer returns either `Guid.Empty` or `false`
5. All `IAfterProcess<TEntity>` hooks are executed

#### Entity deleter service

The entity deleter service is similar to the standard action service in that it accepts an input and returns a `bool` to indicate the success. For that reason, it also runs hooks in the standard order of operations. Its only method is `Delete(Guid)`

1. The entity is fetched from the database using the supplied ID. If the entity is not found, the operation returns `false`
2. All `IAccessValidator<TEntity>` hooks are executed
3. All `IStateValidator<TEntity>` hooks are executed
4. All `IBeforeProcess<TEntity>` hooks are executed
5. The entity is deleted from the database. If this throws, the operation returns `false`
6. `IAfterProcess<TEntity>` hooks are executed