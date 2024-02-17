---
pageTitle: StyleResource
blurb: "Documentation for the StyleResource class"
tags:
  - api
---

# `StyleResource` class

The `StyleResource` class contains the data needed to create an HTML `<link>` tag. The Sienar `/_SienarRazorLayout.cshtml` file uses `IStyleProvider` to create one `<link>` tag for each `StyleResource` in the `IStyleProvider`.

The `StyleResource` class cannot be used to output internal CSS. Sienar does not support internal CSS.

## Instance properties

### `Href`

This `string` property represents the `<link>` tag's `href` attribute. Any valid web URL can be used.

### `CrossOriginMode`

This property contains a `CrossOriginMode?`, which is an enum representing valid values for the `crossorigin` attribute. If `null`, the attribute is omitted altogether. See the [CrossOriginMode doc page](/devs/api/CrossOriginMode) for information on the enum's values.

### `ReferrerPolicy`

This property contains a `ReferrerPolicy?`, which is an enum representing valid values for the `referrerpolicy` attribute. If `null`, the attribute is omitted altogether. See the [ReferrerPolicy doc page](/devs/api/ReferrerPolicy) for information on the enum's values.

### `Integrity`

This `string` property represents the value of the stylesheet's `integrity` attribute. If `null`, the attribute is omitted altogether.

## Implicit operators

### `StyleResource(string)`

This operator creates a new `StyleResource` that uses the supplied `string` as its `Href` property.