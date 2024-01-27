using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sienar.Infrastructure.Entities;

namespace Sienar.Identity;

public class ForgotPasswordRequest : Honeypot
{
	[Required]
	[DisplayName("Username or email")]
	public string AccountName { get; set; } = string.Empty;
}