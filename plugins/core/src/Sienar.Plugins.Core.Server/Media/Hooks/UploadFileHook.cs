using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure;
using Sienar.Hooks;

namespace Sienar.Media.Hooks;

public class UploadFileHook : IBeforeAction<Upload>
{
	private readonly IMediaManager _mediaManager;
	private readonly ILogger<UploadFileHook> _logger;
	private readonly INotifier _notifier;
	public UploadFileHook(
		IMediaManager mediaManager,
		ILogger<UploadFileHook> logger,
		INotifier notifier)
	{
		_mediaManager = mediaManager;
		_logger = logger;
		_notifier = notifier;
	}

	/// <inheritdoc />
	public async Task Handle(Upload entity, ActionType action)
	{
		// Only upload on create
		if (action != ActionType.Create) return;

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
			_notifier.Error("Failed to write media file");
		}
	}
}