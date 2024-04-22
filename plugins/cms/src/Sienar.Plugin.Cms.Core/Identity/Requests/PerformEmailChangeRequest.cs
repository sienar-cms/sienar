using System;
using System.ComponentModel.DataAnnotations;

namespace Sienar.Identity.Requests;

public class PerformEmailChangeRequest
{
	[Required]
	public Guid UserId { get; set; }

	[Required]
	public Guid VerificationCode { get; set; }
}