using System;
using Sienar.Infrastructure;
using Sienar.Plugins;

namespace Sienar.Configuration;

/// <summary>
/// Registers an <see cref="IConfigurer{TOptions}"/> to configure the <see cref="SienarAppBuilder"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AppConfigurerAttribute : Attribute
{
	/// <summary>
	/// A configurer for the <see cref="SienarAppBuilder"/>
	/// </summary>
	public Type Configurer { get; }

	/// <summary>
	/// Creates a new instance of <c>AppConfigurerAttribute</c>
	/// </summary>
	/// <param name="configurer">The type of the configurer</param>
	public AppConfigurerAttribute(Type configurer)
	{
		Configurer = configurer;
	}
}
