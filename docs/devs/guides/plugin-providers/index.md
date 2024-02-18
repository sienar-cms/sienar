---
pageTitle: "Customizing Sienar using plugin providers"
blurb: "A guide to customizing Sienar using plugin providers"
pageNumber: 1
tags:
  - plugin-providers
  - guides
---

# Customizing Sienar using plugin providers

Sienar provides plugins with a number of "providers", which are scoped services that help configure specific parts of the Sienar application on each request. Providers are scoped to the current request because Sienar supports having different [sub-apps](/devs/guides/sub-apps) at different endpoints, so providers need to be re-populated per page load.

## Available providers

Use `IComponentProvider` to [override parts of the Sienar UI](/devs/guides/plugin-providers/overriding-ui-components).

Use `IMenuProvider` to [add menu items](/devs/guides/plugin-providers/adding-menu-items) to an app or sub-app. You also use `IMenuProvider` to [add dashboard items](/devs/guides/plugin-providers/adding-dashboard-items) to an app or sub-app.

Use `IRoutableAssemblyProvider` to [add routable pages](/devs/guides/plugin-providers/adding-routable-pages) to an app or sub-app.

Use `IScriptProvider` to [add JS files](/devs/guides/plugin-providers/adding-js) to an app or sub-app.

Use `IStyleProvider` to [add CSS files](/devs/guides/plugin-providers/adding-css) to an app or sub-app.