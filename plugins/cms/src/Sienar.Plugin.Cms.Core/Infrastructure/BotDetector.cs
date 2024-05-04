using Microsoft.Extensions.Logging;
using Sienar.Infrastructure.Data;

namespace Sienar.Infrastructure;

public class BotDetector : IBotDetector
{
	private readonly ILogger<BotDetector> _logger;

	public BotDetector(ILogger<BotDetector> logger)
	{
		_logger = logger;
	}

	/// <inheritdoc />
	public bool IsSpambot(Honeypot honeypot)
	{
		_logger.LogInformation(
			"Form submission completed in {time} seconds",
			honeypot.TimeToComplete.TotalSeconds);

		var isSpambot = !string.IsNullOrEmpty(honeypot.SecretKeyField);

		if (isSpambot)
		{
			_logger.LogError(
				"Spambot detected! Value passed to honeypot was '{value}'",
				honeypot.SecretKeyField);
		}

		return isSpambot;
	}
}