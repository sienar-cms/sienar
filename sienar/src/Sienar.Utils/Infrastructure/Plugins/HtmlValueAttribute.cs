using System;

namespace Sienar.Infrastructure.Plugins;

[AttributeUsage(AttributeTargets.Field)]
public class HtmlValueAttribute : Attribute
{
	public string Value { get; }
	public HtmlValueAttribute(string value) => Value = value;
}