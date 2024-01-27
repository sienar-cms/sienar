#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sienar.Identity;

public class InitiateEmailChangeRequest
{
	[Required]
	[DisplayName("Email")]
	[EmailAddress]
	public string Email { get; set; }

	[Required]
	[DisplayName("Confirm email")]
	[Compare("Email", ErrorMessage = "The email addresses do not match")]
	public string ConfirmEmail { get; set; }

	[Required]
	[DisplayName("Confirm password")]
	[DataType(DataType.Password)]
	public string ConfirmPassword { get; set; }
}