using System;
using System.ComponentModel;
using System.Reflection;
using Sienar.Infrastructure.Plugins;

namespace Sienar.Extensions;

public static class EnumExtensions
{
	/// <summary>
	/// Gets the HTML-expected value of <see cref="ReferrerPolicy"/> and <see cref="CrossOriginMode"/> members
	/// </summary>
	/// <param name="self">the referrer policy or cross-origin mode to get a value for</param>
	/// <returns>the value if the enum is not null and the value is defined, else null</returns>
	public static string? GetHtmlValue(this Enum? self)
	{
		return self?
			.GetType()
			.GetField(self.ToString())?
			.GetCustomAttribute<HtmlValueAttribute>()
			?.Value;
	}

	public static string GetDescription(this Enum self)
	{
		var stringified = self.ToString();
		var a = self
			.GetType()
			.GetField(stringified)
			?.GetCustomAttribute<DescriptionAttribute>();
		return a?.Description ?? stringified;
	}
}