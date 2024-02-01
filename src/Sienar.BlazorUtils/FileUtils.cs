using System;
using System.IO;

namespace Sienar;

public static class FileUtils
{
	public static string BaseAppFolderName { get; set; } = "";

	public static string GetLocalAppDataDirectory()
		=> Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

	public static string GetDesktopDirectory()
		=> Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

	public static string GetBaseRelativePath(string sqliteFilename)
		=> Path.Combine(BaseAppFolderName, sqliteFilename);

	public static void EnsureDirectoryExists(string dirname)
	{
		if (!Directory.Exists(dirname))
		{
			Directory.CreateDirectory(dirname);
		}
	}
}