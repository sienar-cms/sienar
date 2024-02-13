---
pageTitle: "Configuring Sienar"
blurb: "A guide to configuring Sienar"
pageNumber: 1
tags:
  - guides
---

# Configuring Sienar

Sienar relies on a number of `IOptions<T>` configurations in order to operate. Sienar provides sensible defaults for most of these properties, but some require developer input.

## A note on configuration

Sienar detects when a configuration has already been applied and skips applying its own. For any configuration that is not applied, Sienar expects entries in a well-known `appsettings.json` JSON configuration file. The schema of that file looks like this:

```json
{
    "Sienar": {
        "Core": {},
        "Email": {
            "Sender": {},
            "IdentityEmailSubjects": {}
        },
        "Login": {}
    }
}
```

For any of these configuration sections, you can either supply values via JSON in the `appsettings.json` schema above, or you can supply your own values using the `IServiceCollection.Configure()` overload of your choice. You can configure values using environment variables, application secrets, or any other technique you choose - as long as an `IOptions<T>` is added to the DI container, Sienar will detect your configuration and refrain from adding its own.

## Configuring Sienar core (`Sienar:Core`)

Sienar core is configured using an instance of [SienarOptions](/devs/api/SienarOptions), which has several properties used for configuring features of core Sienar.

To enable sending email from your site, set `SienarOptions.EnableEmail` to `true`. It defaults to `false`. If you want to send email from your application, you will need to supply an implementation of `IEmailSender` because the default Sienar email sender silently does nothing. The [Sienar MailKit plugin](/devs/plugins/mailkit) provides an implementation of `IEmailSender` that sends SMTP email via MailKit, which requires further configuration (see the linked documentation for more information).

To enable new user registration on your site, set `SienarOptions.RegistrationOpen` to `true`. It defaults to `false`.

To configure your site's display name in various places throughout the Sienar UI, set `SienarOptions.SiteName`. It defaults to `string.Empty`.

To configure your site's URL, which is primarily used when sending email (such as password reset links), set `SienarOptions.SiteUrl`. It defaults to `string.Empty`. Unlike WordPress, Sienar uses root-relative links internally, so if your site doesn't send email, this can safely be blank.

## Configuring Sienar email sender information (`Sienar:Email:Sender`)

If Sienar is configured to send email, it needs to know what sender identifiers need to be set. This is done with an instance of [EmailSenderOptions](/devs/api/EmailSenderOptions). **NOTE**: Sienar does not provide defaults for any of these values. If you want to send email, you need to supply values for this configuration class.

The email address used to send mail is configured using `EmailSenderOptions.FromAddress`.

The name shown in the "from" section is configured using `EmailSenderOptions.FromName`.

The email signature is configured using `EmailSenderOptions.Signature`.

## Configuring Sienar account email subjects (`Sienar:Email:IdentityEmailSubjects`)

If Sienar is configured to send email, you can configure the email subjects of account-related emails with an instance of [IdentityEmailSubjectOptions](/devs/api/IdentityEmailSubjectOptions).

The email subject for a new user's welcome email is configured using `IdentityEmailSubjectOptions.WelcomeEmail`. It defaults to `Please confirm your account`.

The email subject for a user's email change request is configured using `IdentityEmailSubjectOptions.EmailChange`. It defaults to `Confirm your new email address`.

The email subject for a user's password reset request is configured using `IdentityEmailSubjectOptions.PasswordReset`. It defaults to `Password reset`.

## Configuring Sienar login (`Sienar:Login`)

Sienar enables sevelopers to set login options with an instance of [LoginOptions](/devs/api/LoginOptions).

A persistent login is one in which the user has ticked the "Remember me" checkbox when logging in. The duration of a persistent login is measured in days with the `LoginOptions.PersistentLoginDuration` property, which is a `double`. The default value is `30` days.

A transient login is one in which the user has *not* ticked the "Remember me" checkbox when logging in. The duration of a transient login is measured in hours with the `LoginOptions.TransientLoginDuration` property, which is a `double`. The default value is `2` hours.

Devs can decide whether users must first confirm their account before they can start logging in. This is managed with the `LoginOptions.RequireConfirmedAccount` property, which is a `bool`. The default value is `true`.

Devs can decide how long a user should be locked out following multiple failed login attempts. This is managed with the `LoginOptions.LockoutTimespan` property, which is a `TimeSpan`. The default value is 15 minutes.

Devs can decide how many failed attempts a user should get before locking out their account. This is managed with the `LoginOptions.MaxFailedLoginAttempts` property, which is an `int`. The default value is `3`. After `3` failed login attempts, the user's account will be locked for a duration matching `LoginOptions.LockoutTimespan`. This value cannot currently be disabled for security reasons, so setting a value less than `1` is functionally similar to setting a value of `1`. 