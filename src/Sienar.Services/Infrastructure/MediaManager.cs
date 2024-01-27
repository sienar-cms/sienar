using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Sienar.Infrastructure;

public class MediaManager : IMediaManager
{
	protected readonly IMediaDirectoryMapper DirectoryMapper;

	public MediaManager(IMediaDirectoryMapper directoryMapper)
	{
		DirectoryMapper = directoryMapper;
	}

	/// <inheritdoc />
	public async Task<bool> WriteFile(string path, IFormFile fileContents)
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
		var baseDir = DirectoryMapper.GetDirectoryPath(type);
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