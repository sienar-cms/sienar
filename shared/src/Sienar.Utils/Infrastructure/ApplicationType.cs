namespace Sienar.Infrastructure;

/// <summary>
/// Provides a platform-agnostic way of indicating the target environment for a Sienar application
/// </summary>
public enum ApplicationType
{
	/// <summary>
	/// Represents applications that run in a web server environment
	/// </summary>
	Server,

	/// <summary>
	/// Represents applications that run in a web browser environment
	/// </summary>
	Client,

	/// <summary>
	/// Represents applications that run in a desktop environment
	/// </summary>
	Desktop,

	/// <summary>
	/// Represents applications that run in a console environment
	/// </summary>
	Console
}
