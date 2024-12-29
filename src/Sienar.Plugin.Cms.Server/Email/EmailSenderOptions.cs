namespace Sienar.Email;

/// <summary>
/// Configures email settings for your application
/// </summary>
public class EmailSenderOptions
{
	/// <summary>
	/// The email address from which to send application email
	/// </summary>
	public required string FromAddress { get; set; }

	/// <summary>
	/// The name to use as the sender of application email
	/// </summary>
	public required string FromName { get; set; }

	/// <summary>
	/// The signature to use in application email
	/// </summary>
	public required string Signature { get; set; }
}
