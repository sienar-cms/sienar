using Sienar.Infrastructure.Entities;

namespace Sienar.Infrastructure;

public interface IBotDetector
{
	/// <summary>
	/// Determines whether a honeypot has caught a bot
	/// </summary>
	/// <param name="honeypot">The honeypot</param>
	/// <returns>whether the honeypot caught a bot</returns>
	bool IsSpambot(Honeypot honeypot);
}