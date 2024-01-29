using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure.Hooks;
using Sienar.Media;

namespace Sienar.Media.Hooks;

public class UploadFileHook : IBeforeUpsert<Upload>
{
	private readonly IMediaManager _mediaManager;
	private readonly ILogger<UploadFileHook> _logger;
	public UploadFileHook(
		IMediaManager mediaManager,
		ILogger<UploadFileHook> logger)
	{
		_mediaManager = mediaManager;
		_logger = logger;
	}

	/// <inheritdoc />
	public async Task<HookStatus> Handle(Upload entity, bool isAdding)
	{
		// Only upload on create
		if (!isAdding) return HookStatus.Success;

		entity.Path = _mediaManager.GetFilename(
			".txt", // TODO: find the actual file extension somehow 
			entity.MediaType);

		try
		{
			await _mediaManager.WriteFile(entity.Path, entity.Contents!);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to write media file");
			return HookStatus.Unknown;
		}

		return HookStatus.Success;
	}
}