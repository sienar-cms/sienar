using System;
using System.IO;
using Sienar.Tools;

namespace Sienar;

public static class SienarUtils
{
	public static void SetupBaseDirectory()
	{
		FileUtils.BaseAppFolderName = Path.Combine(
			Environment.CurrentDirectory,
			"../SienarFiles");
		FileUtils.EnsureDirectoryExists(FileUtils.BaseAppFolderName);
	}
}