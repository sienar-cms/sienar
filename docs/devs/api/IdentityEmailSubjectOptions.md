---
pageTitle: IdentityEmailSubjectOptions
blurb: "Documentation for the IdentityEmailSubjectOptions class"
tags:
  - api
---

# `IdentityEmailSubjectOptions` class

The `IdentityEmailSubjectOptions` class is used to configure account-related email message subjects. Its values can be set either by executing an overload of `IServiceCollection.Configure<IdentityEmailSubjectOptions>()`, or by supplying an object in `appsettings.json` at `Sienar:Email:IdentityEmailSubjects`.

## Instance properties

### `WelcomeEmail`

This property is used to define the subject for a new user's welcome email. Defaults to `Please confirm your account`.

### `EmailChange`

This property is used to define the subject for a user's email change request email. Defaults to `Confirm your new email address`.

### `PasswordReset`

This property is used to define the subject for a user's password reset request email. Defaults to `Password reset`.