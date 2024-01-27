#nullable disable

namespace Sienar.Email;

public class EmailOptions
{
	public string FromAddress { get; set; }
	public string FromName { get; set; }
	public string ApplicationUrl { get; set; }
	public string DashboardUrl { get; set; }
	public string Signature { get; set; }
}
