namespace Sienar.Media;

public interface IMediaDirectoryMapper
{
	/// <summary>
	/// Maps a <see cref="MediaType"/> to a directory path
	/// </summary>
	/// <param name="type">The <see cref="MediaType"/> to map</param>
	/// <returns>the mapped directory path</returns>
	string GetDirectoryPath(MediaType type);
}