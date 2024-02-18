---
pageTitle: Home
blurb: "Sienar: an application development meta-framework built on top of Blazor Server and MudBlazor"
---

# Sienar
## An application development framework built on top of Blazor Server and MudBlazor

Sienar is a framework for building desktop and web applications. When building web applications with Sienar, a small CMS is also included.

## Philosophy

Sienar is designed to allow the developer to build hookable and modular applications. The basic design philosophy was loosely inspired by WordPress' hooks and plugin systems; Sienar offers similar functionality via a variety of interfaces.

### Sienar Web

Sienar is built on top of Blazor Server and the MudBlazor component library. These are opinionated decisions intended to speed up the initial application development process by avoiding [decision paralysis](https://en.wikipedia.org/wiki/Analysis_paralysis). While Blazor Server and MudBlazor both have drawbacks, neither option is objectively worse when weighed against the alternatives. While Blazor Server requires additional server resources, it also greatly simplifies application architecture and overall decreases the time needed to develop features; Blazor Server apps also don't require more resources on the low end, so by the time performance becomes a problem, an application should have the financial backing to scale out or refactor. And while MudBlazor may lack robust customization options, the default components are attractive, performant, functional, and - most importantly - free. MudBlazor components are only forced in the default layout and dashboard, so if you want to use different components for other parts of your app, you can. You also don't have to use the Sienar-provided UI at all if you don't want to.

### Sienar Desktop

When developing for desktop, Sienar utilizes [Electron.NET](https://github.com/ElectronNET/Electron.NET). This is, again, an opinionated decision merely meant to force an option among a wealth of .NET desktop development options, few of which are as cross-platform as Electron. I was hesitant to join in the over-utilization of Electron in desktop development, but I'm far more hesitant about the long-term viability of .NET MAUI given Microsoft's poor track record with UI frameworks. Since no other UI framework supports Blazor as a first-class citizen, I decided that Electron.NET was the best decision for Sienar, even though I try to avoid Electron as a general rule.