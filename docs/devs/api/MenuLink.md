---
pageTitle: MenuLink
blurb: "Documentation for the MenuLink class"
tags:
  - api
---

# `MenuLink` class

The `MenuLink` class is used to contain all the data needed to create a menu link. `MenuLink` instances should be processed by an `IMenuGenerator` before being used by developers. This will ensure that the current user meets all requirements for the `MenuLink` before it is rendered. (The `IMenuGenerator` excludes `MenuLink` instances for which the user does not meet the requirements to view.)

The `MenuLink` class is derived from [DashboardLink](/devs/api/DashboardLink). Most of its properties come from `DashboardLink`.

## Instance properties

### `Sublinks`

This `List<MenuLink>?` property optionally defines child links.

The `Sublinks` property indicates that the current `MenuLink` should be treated as the opening node of a sub-menu with children defined by this `Sublinks` property. Each item in `Sublinks` then becomes its own submenu item. `Sublinks` can contain their own nested `Sublinks` with no logical restriction on depth, but be aware that space is likely to become cramped beyond two or three layers deep.

`Sublinks` can provide additional restrictions beyond their parent `MenuLink` instance. For example, a parent `MenuLink` instance can have a requirement of `MenuLink.RequireLoggedIn = true`. If the user is not logged in, none of that link's `Sublinks` will be rendered. However, if the user is logged in properly, `Sublinks` can define additional requirements. For example, a sublink can require that a user be an administrator, and it will not render if the user is not an administrator even if the parent link or other sibling sublinks do render.