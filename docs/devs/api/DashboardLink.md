---
pageTitle: DashboardLink
blurb: "Documentation for the DashboardLink class"
tags:
  - api
---

# `DashboardLink` class

The `DashboardLink` class is used to contain all the data needed to create a dashboard link. `DashboardLink` instances should be processed by an `IDashboardGenerator` before being used by developers. This will ensure that the current user meets all requirements for the `DashboardLink` before it is rendered. (The `IDashboardGenerator` excludes `DashboardLink` instances for which the user does not meet the requirements to view.)

## Instance properties

### `Text`

This string property is required. It represents the display text of the generated link.

### `Url`

This string property represents the `href` attribute of the generated link.

### `Icon`

This optional string property represents the icon to display with the generated link. In the context of a default Sienar dashboard link, this should be any valid SVG string, such as the code of a [MudBlazor icon](https://mudblazor.com/features/icons#icons). However, if you plan to consume a `DashboardLink` from your own code, it can be any string used to identify an icon, such as a FontAwesome CSS class like `fas fa-times`.

### `RequireLoggedIn`

This `bool` property indicates that a user must be logged in to view the `DashboardLink`. For example, a user must be logged in to see the log out link, so Sienar uses `RequireLoggedIn` to ensure that the log out link is only displayed to logged in users.

### `RequireLoggedOut`

This `bool` property indicates that a user must be logged out to view the `DashboardLink`.

For example, a user should only see the log in link if they are logged out, so Sienar uses `RequireLoggedOut` to ensure that the log in link is only displayed to logged out users.

### `Roles`

This `IEnumerable<string>?` property indicates a group of roles that a user must be a member of in order to view the link.

For example, a user should only see the link to the user administration page if they are in the `Administrator` role, so Sienar sets `Roles = [ Roles.Admin ]` when creating the user administration page link.

### `AllRolesRequired`

This `bool` property indicates that a user needs to satisfy all roles stored in the `Roles` property in order to view a link. Defaults to `true`. If `false`, a user can view a link by being in only a single role in the `Roles` property.

For example, if you want to create a page for editing entities in your custom plugin, you might want to restrict the link to administrators and editors. You would do this by setting `Roles = [ Roles.Admin, Roles.Editor ]` and `AllRolesRequired = false`. In this way, the user will be able to view the link if they are in either the administrator or editor roles. The `Roles` enumerable does not become exclusive if `AllRolesRequired = false`, though - users who are both administrators *and* editors will still be able to view the link.