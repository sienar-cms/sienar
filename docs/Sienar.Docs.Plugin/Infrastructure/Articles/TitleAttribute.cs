using System;

namespace Sienar.Infrastructure.Articles;

[AttributeUsage(AttributeTargets.Class)]
public class TitleAttribute : Attribute
{
	public string Title { get; }
	public TitleAttribute(string title) => Title = title;
}