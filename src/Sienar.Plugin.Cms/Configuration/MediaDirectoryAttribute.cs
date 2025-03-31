using System;

namespace Sienar.Configuration;

[AttributeUsage(AttributeTargets.Field)]
public class MediaDirectoryAttribute : Attribute
{
	public string Directory { get; }

	public MediaDirectoryAttribute(string directory)
	{
		Directory = directory;
	}
}