#nullable disable

namespace Sienar.Email;

public class EmailSenderOptions
{
	/// <summary>
	/// The email address from which to send application email
	/// </summary>
	public string FromAddress { get; set; }

	/// <summary>
	/// The name to use as the sender of application email
	/// </summary>
	public string FromName { get; set; }

	/// <summary>
	/// The signature to use in application email
	/// </summary>
	public string Signature { get; set; }
}
