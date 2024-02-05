using System;

namespace Sienar.Infrastructure.Articles;

[AttributeUsage(AttributeTargets.Class)]
public class SeriesAttribute : Attribute
{
	public string Series { get; }

	/// <inheritdoc />
	public SeriesAttribute(string series) => Series = series;
}