using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sienar.Data;

namespace Sienar.Identity.Requests;

public class ForgotPasswordRequest : Honeypot
{
	[Required]
	[DisplayName("Username or email")]
	public string AccountName { get; set; } = string.Empty;
}