using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sienar.Identity.Requests;

public class InitiateEmailChangeRequest
{
	[Required]
	[DisplayName("Email")]
	[EmailAddress]
	public string Email { get; set; } = string.Empty;

	[Required]
	[DisplayName("Confirm email")]
	[Compare("Email", ErrorMessage = "The email addresses do not match")]
	public string ConfirmEmail { get; set; } = string.Empty;

	[Required]
	[DisplayName("Confirm password")]
	[DataType(DataType.Password)]
	public string ConfirmPassword { get; set; } = string.Empty;
}