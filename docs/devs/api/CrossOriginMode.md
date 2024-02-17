---
pageTitle: CrossOriginMode
blurb: "Documentation for the CrossOriginMode enum"
tags:
  - api
---

# `CrossOriginMode` enum

The `CrossOriginMode` enum represents the possible values of the `crossorigin` HTML attribute. Sienar uses this enum when creating script and style resources.

For information on individual values of the `crossorigin` attribute, see [crossorigin on MDN](https://developer.mozilla.org/en-US/docs/Web/HTML/Attributes/crossorigin).

## Members

### `None`

The `None` member represents `crossorigin=""`.

### `Anonymous`

The `Anonymous` member represents `crossorigin="anonymous"`.

### `UseCredentials`

The `UseCredentials` member represents `crossorigin="use-credentials"`.