namespace Sienar.Configuration;

public class SienarOptions
{
	/// <summary>
	/// Whether Sienar should send email notifications or not
	/// </summary>
	public bool EnableEmail { get; set; } = true;

	/// <summary>
	/// Whether Sienar should allow the registration of new users or not
	/// </summary>
	public bool RegistrationOpen { get; set; } = true;
}