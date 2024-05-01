namespace Sienar.Infrastructure.Data;

/// <summary>
/// Represents a serialized notification for transmission across a REST API
/// </summary>
public class Notification
{
	/// <summary>
	/// The message of the notification
	/// </summary>
	public required string Message { get; set; }

	/// <summary>
	/// The type of the notification
	/// </summary>
	public required NotificationType Type { get; set; }
}