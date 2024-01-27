using System;
using System.ComponentModel;
using System.Reflection;

namespace Sienar;

public static class SienarBlazorUtils
{
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