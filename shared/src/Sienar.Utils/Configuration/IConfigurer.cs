using System;

namespace Sienar.Configuration;

/// <summary>
/// Used by Sienar to configure a service or middleware that is generally configured by an <see cref="Action{T}"/>
/// </summary>
/// <typeparam name="TOptions">the type of the configuration options class to configure</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface IConfigurer<TOptions> where TOptions : class
{
	/// <summary>
	/// Configures an instance of <c>TOptions</c>
	/// </summary>
	/// <param name="options">the instance of <c>TOptions</c> to configure</param>
	void Configure(TOptions options);
}