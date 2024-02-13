---
pageTitle: LoginOptions
blurb: "Documentation for the LoginOptions class"
tags:
  - api
---

# `LoginOptions` class

The `LoginOptions` class is used to tell Sienar how to handle various login-related scenarios. Its values can be set either by executing an overload of `IServiceCollection.Configure<LoginOptions>()`, or by supplying an object in `appsettings.json` at `Sienar:Login`.

## Instance properties

### `PersistentLoginDuration`

This `double` represents how long a persistent ("remember me" checked) login session lasts in days. Defaults to `30` days.

### `TransientLoginDuration`

This `double` represents how long a transient ("remember me" not checked) login session lasts in hours. Defaults to `2` days.

### `RequireConfirmedAccount`

This `bool` represents whether to require users to confirm their account prior to logging in. Defaults to `true`.

### `LockoutTimespan`

This `TimeSpan` represents how long a user account is locked out after multiple failed login attempts. Defaults to 15 minutes.

### `MaxFailedLoginAttempts`

This `int` represents how many times a user can fail to log in before their account is locked. Defaults to `3` failures.