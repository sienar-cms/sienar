---
_layout: landing
---

# Sienar
## An application development framework built on top of Blazor Server and MudBlazor

Sienar is a framework for building desktop and web applications. When building web applications with Sienar, a small CMS is also included.

## Philosophy

Sienar is designed to allow the developer to build hookable and modular applications. The basic design philosophy was loosely inspired by WordPress' hooks and plugin systems; Sienar offers similar functionality via a variety of interfaces.

### Sienar Web

Sienar is built on top of Blazor United and the MudBlazor component library. These are opinionated decisions intended to speed up the initial application development process by avoiding [decision paralysis](https://en.wikipedia.org/wiki/Analysis_paralysis). While Blazor and MudBlazor both have drawbacks, neither option is objectively worse when weighed against the alternatives. While Blazor requires additional server resources, it also greatly simplifies application architecture and overall decreases the time needed to develop features; Blazor apps also don't require more resources on the low end, so by the time performance becomes a problem, an application should have the financial backing to scale out or refactor. And while MudBlazor may lack robust customization options, the default components are attractive, performant, functional, and - most importantly - free. MudBlazor components are only forced in the default layout and dashboard, so if you want to use different components for other parts of your app, you can. You also don't have to use the Sienar-provided UI at all if you don't want to.

### Sienar Desktop

When developing for desktop, Sienar utilizes .NET MAUI with `BlazorWebView`. Because MAUI is the successor to Xamarin, which Microsoft spent significant money to acquire, we feel that MAUI will most likely have long-term attention from Microsoft. We may explore other options later on if necessary, but for now, .NET MAUI is our choice for desktop apps.

### Other use cases

Sienar provides utilities for web apps and desktop apps via NuGet packages, but you can also build your own solutions as you see fit. For example, you might consider building desktop apps using Electron.NET and Sienar Web, which is a solution that a pre-alpha version of Sienar used before porting desktop apps to .NET MAUI. You can also build your own application framework on top of the base Sienar utilities if you want. Sienar keeps very little closed off from developers, so the possibilities are almost endless.