---
pageTitle: Getting started
blurb: "An introduction to developing with Sienar"
pageNumber: 1
tags:
  - introduction
---

# Getting started

There are two ways to create a new Sienar application: using the Sienar `dotnet new` project templates, or installing Sienar manually into a .NET project.

## Using Sienar templates

To install the Sienar templates, run:

```bash
dotnet new install Sienar.Templates
```

Then, to create a new Sienar application, run:

```bash
dotnet new sienarweb -o <project_name>
```

The Sienar template is pretty beefy because it sets up everything that you need out of the box. It includes a `DbContext`, migrations that add all necessary database entries for Sienar to work, a default sub-app plugin that executes if no other plugin has executed, a blank `MudTheme` for you to configure, a main menu for your default sub-app plugin, and more. All you need to do is run the generated template and you have a working Sienar site - the generated template has SQLite configured and migrates the database on startup by default.

## Installing manually

If you don't want to use the Sienar template (we understand that it does a *lot*), you can always install Sienar manually and configure it the way you want from the ground up.

You will want to install one of three main Sienar packages, depending on what you want to use Sienar for:

- If you want to develop a desktop application, install `Sienar.DesktopUtils`
- If you want to develop a web application while providing 100% of your own UI, install `Sienar.Blazor`
- If you want to develop a web application with an existing admin dashboard, install `Sienar.Cms`

### 1. Create a blank web application

Open a command prompt and navigate to the directory where you want to create your project, then run:

```bash
dotnet new web -o SienarExample # or whatever you want your project called
```

### 2. Install required dependencies

Now, you need to install the appropriate Sienar nuget package in your new project (see above)

```bash
cd SienarExample
dotnet add package Sienar.Cms
dotnet add package Microsoft.EntityFrameworkCore.Sqlite # feel free to change this for your app - this is just an example. Sienar only uses EF for queries, so nothing will break
dotnet add package Microsoft.EntityFrameworkCore.Design # necessary for ef-tools
```

### 3. Add a `DbContext`

Open the project in your favorite code editor, then add a new class called `AppDbContext`. Sienar has several entities that it requires in the database, so you can either add those entities manually or inherit from `SienarDbContext`, which has all the entities Sienar needs. If you want the best of both worlds, you can implement the `ISienarDbContext` interface, which guarantees you have the entities Sienar needs while giving you complete control over the context class. Create the following implementation:

```csharp
using Microsoft.EntityFrameworkCore;
using Sienar;

namespace SienarExample;

public class AppDbContext : SienarDbContext
{
	/// <inheritdoc />
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite("Data Source=sienarexample.db");
	}
}
```

Now, you need to get the database set up:

```bash
dotnet ef migrations add Initial
dotnet ef database update
```

### 4. Update `Program.cs`

Open `<project-name>/Program.cs` in your favorite code editor and completely delete its contents, then add the following:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Plugins;
using SienarExample;

await SienarServerAppBuilder
	.Create<AppDbContext>(
		args,
		dbContextLifetime: ServiceLifetime.Transient)
	.AddPlugin<SienarBlazorPlugin>()
	.AddPlugin<SienarCmsPlugin>()
	.Build()
	.RunAsync();
```

### Test the application

Now you can run the application. Once running, visit the app at `http://localhost:<port>/dashboard`. You should be redirected to the login page. If this happens, the application is running correctly! (If you try to visit the app root at `http://localhost:<port>`, you'll get an exception because of the way Sienar registers assemblies that contain routable Blazor components.)