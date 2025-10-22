using System;
using System.IO;
using System.Threading.Tasks;

namespace Sienar.Media;

public class MediaManager : IMediaManager
{
	private readonly IMediaDirectoryMapper _directoryMapper;

	public MediaManager(IMediaDirectoryMapper directoryMapper)
	{
		_directoryMapper = directoryMapper;
	}

	/// <inheritdoc />
	public async Task<bool> WriteFile(string path, Stream fileContents)
	{
		var dirname = Path.GetDirectoryName(path);
		if (!Directory.Exists(dirname))
		{
			Directory.CreateDirectory(dirname!);
		}

		await using var file = File.Create(path);
		await fileContents.CopyToAsync(file);
		await file.FlushAsync();
		return true;
	}

	/// <inheritdoc />
	public Task<bool> DeleteFile(string path)
	{
		File.Delete(path);
		return Task.FromResult(true);
	}

	/// <inheritdoc />
	public string GetFilename(string? fileExtension, MediaType type)
	{
		var baseDir = _directoryMapper.GetDirectoryPath(type);
		var id = Guid.NewGuid();

		if (string.IsNullOrEmpty(fileExtension))
		{
			fileExtension = string.Empty;
		}
		else
		{
			if (!fileExtension.StartsWith('.'))
			{
				fileExtension = $".{fileExtension}";
			}
		}

		return $"{baseDir}/{id}{fileExtension}";
	}
}