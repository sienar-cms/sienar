---
pageTitle: MenuLink
blurb: "Documentation for the MenuLink class"
tags:
  - api
---

# `MenuLink` class

The `MenuLink` class is used to contain all the data needed to create a menu link or a dashboard link. `MenuLink` instances should be processed by an `IMenuGenerator` before being used by developers. This will ensure that the current user meets all requirements for the `MenuLink` before it is rendered. (The `IMenuGenerator` excludes `MenuLink` instances for which the user does not meet the requirements to view.)

## Instance properties

### `Text`

This string property is required. It represents the display text of the generated link.

### `Url`

This string property represents the `href` attribute of the generated link. If a `MenuLink` has sublinks, this value is ignored.

### `Icon`

This optional string property represents the icon to display with the generated link. In the context of a default Sienar menu link or dashboard link, this should be any valid SVG string, such as the code of a [MudBlazor icon](https://mudblazor.com/features/icons#icons). However, if you plan to consume a `MenuLink` from your own code, it can be any string used to identify an icon, such as a FontAwesome CSS class like `fas fa-times`.

### `RequireLoggedIn`

This `bool` property indicates that a user must be logged in to view the `MenuLink`. For example, a user must be logged in to see the log out link, so Sienar uses `RequireLoggedIn` to ensure that the log out link is only displayed to logged in users.

`RequireLoggedIn` does not cascade directly to `Sublinks`, but if a `MenuLink` fails the `RequireLoggedIn` check, its `Sublinks` will never render.

### `RequireLoggedOut`

This `bool` property indicates that a user must be logged out to view the `MenuLink`.

For example, a user should only see the log in link if they are logged out, so Sienar uses `RequireLoggedOut` to ensure that the log in link is only displayed to logged out users.

`RequireLoggedOut` does not cascade directly to `Sublinks`, but if a `MenuLink` fails the `RequireLoggedOut` check, its `Sublinks` will never render.

### `Roles`

This `IEnumerable<string>?` property indicates a group of roles that a user must be a member of in order to view the link.

For example, a user should only see the link to the user administration page if they are in the `Administrator` role, so Sienar sets `Roles = [ Roles.Admin ]` when creating the user administration page link.

`Roles` do not cascade directly to `Sublinks`, but if a `MenuLink` fails a `Roles` requirement, its `Sublinks` will never render.

### `AllRolesRequired`

This `bool` property indicates that a user needs to satisfy all roles stored in the `Roles` property in order to view a link. Defaults to `true`. If `false`, a user can view a link by being in only a single role in the `Roles` property.

For example, if you want to create a page for editing entities in your custom plugin, you might want to restrict the link to administrators and editors. You would do this by setting `Roles = [ Roles.Admin, Roles.Editor ]` and `AllRolesRequired = false`. In this way, the user will be able to view the link if they are in either the administrator or editor roles. The `Roles` enumerable does not become exclusive if `AllRolesRequired = false`, though - users who are both administrators *and* editors will still be able to view the link.

### `Sublinks`

This `IEnumerable<MenuLink>?` property optionally defines child links.

In `MenuLink` instances intended for menus, the `Sublinks` property indicates that the current `MenuLink` should be treated as the opening node of a sub-menu with children defined by this `Sublinks` property. Each item in `Sublinks` then becomes its own submenu item. `Sublinks` can contain their own nested `Sublinks` with no logical restriction on depth, but be aware that space is likely to become cramped beyond two or three layers deep.

`Sublinks` can provide additional restrictions beyond their parent `MenuLink` instance. For example, a parent `MenuLink` instance can have a requirement of `MenuLink.RequireLoggedIn = true`. If the user is not logged in, none of that link's `Sublinks` will be rendered. However, if the user is logged in properly, `Sublinks` can define additional requirements. For example, a sublink can require that a user be an administrator, and it will not render if the user is not an administrator even if the parent link or other sibling sublinks do render.

**NOTE**: `MenuLink` instances intended for use in dashboards should not define `Sublinks` because the property is ignored.