#nullable disable

using System;
using System.ComponentModel.DataAnnotations;

namespace Sienar.Identity;

public class ConfirmAccountRequest
{
	[Required]
	public Guid UserId { get; set; }

	[Required]
	public Guid VerificationCode { get; set; }
}