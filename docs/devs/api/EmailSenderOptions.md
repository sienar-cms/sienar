---
pageTitle: EmailSenderOptions
blurb: "Documentation for the EmailSenderOptions class"
tags:
  - api
---

# `EmailSenderOptions` class

The `EmailSenderOptions` class is used to provide email sender configuration values. Its values can be set either by executing an overload of `IServiceCollection.Configure<EmailSenderOptions>()`, or by supplying an object in `appsettings.json` at `Sienar:Email:Sender`.

## Instance properties

### `FromAddress`

This `string` is used as the sender's email address field.

### `FromName`

This `string` is used as the display name of the sender.

### `Signature`

This `string` is used as the sign-off signature in emails. For example, if an email signs off with:

```text
Sincerely,

The Application Team
```

then in this case, the signature portion would be "The Application Team".