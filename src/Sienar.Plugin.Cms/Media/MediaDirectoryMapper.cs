using System.IO;

namespace Sienar.Media;

public class MediaDirectoryMapper : IMediaDirectoryMapper
{
	/// <inheritdoc />
	public string GetDirectoryPath(MediaType type)
	{
		var cwd = Directory.GetCurrentDirectory();
		var directoryName = type.GetMediaDirectory()
			?? MediaType.Other.GetMediaDirectory();
		return $"{cwd}/media/{directoryName}";
	}
}