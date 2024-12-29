using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sienar.Data;

namespace Sienar.Identity.Requests;

public class LoginRequest : Honeypot
{
	[Required]
	[DisplayName("Username or email")]
	public string AccountName { get; set; } = string.Empty;

	[Required]
	[DisplayName("Password")]
	[DataType(DataType.Password)]
	public string Password { get; set; } = string.Empty;

	[DisplayName("Remember me")]
	public bool RememberMe { get; set; }
}