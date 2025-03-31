using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sienar.Validation;
using Sienar.Data;
using Sienar.Services;

namespace Sienar.Identity.Requests;

public class ResetPasswordRequest : Honeypot, IRequest
{
	[Required]
	public Guid UserId { get; set; }

	[Required]
	public Guid VerificationCode { get; set; }

	[Required]
	[DisplayName("New password")]
	[DataType(DataType.Password)]
	[RequireUpper(ErrorMessage = "Your new password must contain an uppercase letter")]
	[RequireLower(ErrorMessage = "Your new password must contain a lowercase letter")]
	[RequireDigit(ErrorMessage = "Your new password must contain a number")]
	[RequireSpecial(ErrorMessage = "Your new password must contain a special character")]
	public string NewPassword { get; set; } = string.Empty;

	[Required]
	[DisplayName("Confirm new password")]
	[DataType(DataType.Password)]
	[Compare("NewPassword", ErrorMessage = "The passwords do not match")]
	public string ConfirmNewPassword { get; set; } = string.Empty;
}