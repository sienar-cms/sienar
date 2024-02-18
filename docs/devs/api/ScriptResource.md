---
pageTitle: ScriptResource
blurb: "Documentation for the ScriptResource class"
tags:
  - api
---

# `ScriptResource` class

The `ScriptResource` class contains the data needed to create an HTML `<script>` tag. The Sienar `/_SienarRazorLayout.cshtml` file uses `IScriptProvider` to create one `<script>` tag for each `ScriptResource` in the `IScriptProvider`.

The `ScriptResource` class cannot be used to output inline JavaScript. Sienar does not support inline JavaScript.

## Instance methods

### `GetScriptType()`

```csharp
/// <summary>
/// Returns <c>"module"</c> if the script is an ES module, otherwise <c>null</c>
/// </summary>
public string? GetScriptType();
```

This method looks at the `ScriptResource.IsModule` property. If it is `true`, this method returns `"module"`. If it is `false`, this method returns `null`.

## Instance properties

### `Src`

This `string` property represents the `<script>` tag's `src` attribute. Any valid web URL can be used.

### `IsModule`

This `bool` property represents whether the resource is an EcmaScript module. If `true`, the generated `<script>` tag will have an additional attribute of `type="module"`. If `false`, the `<script>` tag will not have a `type` attribute since `type` is only required for modules and inline scripts.

### `IsAsync`

If this `bool` property is `true`, the generated `<script>` tag will have the `async` attribute.

### `ShouldDefer`

If this `bool` property is `true`, the generated `<script>` tag will have the `defer` attribute.

### `CrossOriginMode`

This property contains a `CrossOriginMode?`, which is an enum representing valid values for the `crossorigin` attribute. If `null`, the attribute is omitted altogether. See the [CrossOriginMode doc page](/devs/api/CrossOriginMode) for information on the enum's values.

### `ReferrerPolicy`

This property contains a `ReferrerPolicy?`, which is an enum representing valid values for the `referrerpolicy` attribute. If `null`, the attribute is omitted altogether. See the [ReferrerPolicy doc page](/devs/api/ReferrerPolicy) for information on the enum's values.

### `Integrity`

This `string` property represents the value of the script's `integrity` attribute. If `null`, the attribute is omitted altogether.

## Implicit operators

### `ScriptResource(string)`

This operator creates a new `ScriptResource` that uses the supplied `string` as its `Src` property.