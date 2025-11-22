using System;
using Microsoft.Extensions.DependencyInjection;

namespace Sienar.Infrastructure;

/// <summary>
/// Maps underlying methods of different app builder types so they can be called generically
/// </summary>
public interface IApplicationAdapter
{
	/// <summary>
	/// The specific type of application the adapter represents
	/// </summary>
	ApplicationType ApplicationType { get; }

	/// <summary>
	/// Calls the underlying app builder's <c>Create()</c> method
	/// </summary>
	/// <param name="args">The application startup CLI arguments</param>
	/// <param name="startupServices">The application startup services</param>
	void Create(string[] args, IServiceCollection startupServices);

	/// <summary>
	/// Calls the underlying app builder's <c>Build()</c> method
	/// </summary>
	/// <remarks>
	/// This method is responsible for building the final application. It is also responsible for translating any services between the startup service provider and the runtime service collection. For example, <see cref="Sienar.Menus.MenuProvider"/>, which is configured at application startup in the startup service provider, should be transferred over to the runtime service collection in this method.
	/// </remarks>
	/// <param name="startupServiceProvider">The application startup service container</param>
	/// <returns>The built application</returns>
	object Build(IServiceProvider startupServiceProvider);

	/// <summary>
	/// Adds services to the udnerlying app builder's <see cref="IServiceCollection"/>
	/// </summary>
	/// <param name="configurer">The <see cref="Action"/> to call against the app builder's <see cref="IServiceCollection"/></param>
	void AddServices(Action<IServiceCollection> configurer);
}

/// <summary>
/// Provides indirect access to the underlying application builder
/// </summary>
/// <typeparam name="T">The type of the application builder</typeparam>
public interface IApplicationAdapter<T> : IApplicationAdapter
{
	/// <summary>
	/// The underlying, framework-specific application builder
	/// </summary>
	/// <remarks>
	/// In an ASP.NET app, this will be a <c>WebApplicationBuilder</c>. In a Blazor WASM app, this will be a <c>WebAssemblyHostBuilder</c>. In a MAUI app, this will be a <c>MauiAppBuilder</c>.
	/// </remarks>
	T Builder { get; }
}