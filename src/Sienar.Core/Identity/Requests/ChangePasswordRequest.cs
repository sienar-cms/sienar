#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sienar.Validation;

namespace Sienar.Identity.Requests;

public class ChangePasswordRequest
{
	[Required]
	[DisplayName("New password")]
	[DataType(DataType.Password)]
	[RequireUpper(ErrorMessage = "Your new password must contain an uppercase letter")]
	[RequireLower(ErrorMessage = "Your new password must contain a lowercase letter")]
	[RequireDigit(ErrorMessage = "Your new password must contain a number")]
	[RequireSpecial(ErrorMessage = "Your new password must contain a special character")]
	public string NewPassword { get; set; }

	[Required]
	[DisplayName("Confirm new password")]
	[DataType(DataType.Password)]
	[Compare("NewPassword", ErrorMessage = "The passwords do not match")]
	public string ConfirmNewPassword { get; set; }

	[Required]
	[DisplayName("Enter current password")]
	[DataType(DataType.Password)]
	public string CurrentPassword { get; set; }
}