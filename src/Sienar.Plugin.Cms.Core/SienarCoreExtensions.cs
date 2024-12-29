using System;
using System.Reflection;
using Sienar.Configuration;

namespace Sienar;

public static class SienarCoreExtensions
{
	public static string? GetMediaDirectory(this Enum self)
	{
		var a = self
			.GetType()
			.GetField(self.ToString())
			?.GetCustomAttribute<MediaDirectoryAttribute>();
		return a?.Directory;
	}
}