---
pageTitle: ReferrerPolicy
blurb: "Documentation for the ReferrerPolicy enum"
tags:
  - api
---

# `ReferrerPolicy` enum

The `ReferrerPolicy` enum represents the possible values of the `referrerpolicy` HTML attribute. Sienar uses this enum when creating script and style resources.

For information on individual values of the `referrerpolicy` attribute, see [referrerPolicy on MDN](https://developer.mozilla.org/en-US/docs/Web/API/HTMLScriptElement/referrerPolicy).

## Members

### `NoReferrer`

The `NoReferrer` member represents `referrerpolicy="no-referrer"`.

### `NoReferrerWhenDowngrade`

The `NoReferrerWhenDowngrade` member represents `referrerpolicy="no-referrer-when-downgrade"`.

### `Origin`

The `Origin` member represents `referrerpolicy="origin"`.

### `OriginWhenCrossOrigin`

The `OriginWhenCrossOrigin` member represents `referrerpolicy="origin-when-cross-origin"`.

### `SameOrigin`

The `SameOrigin` member represents `referrerpolicy="same-origin"`.

### `StrictOrigin`

The `StrictOrigin` member represents `referrerpolicy="strict-origin"`.

### `StrictOriginWhenCrossOrigin`

The `StrictOriginWhenCrossOrigin` member represents `referrerpolicy="strict-origin-when-cross-origin"`.

### `UnsafeUrl`

The `UnsafeUrl` member represents `referrerpolicy="unsafe-url"`.