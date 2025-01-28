using System;
using System.ComponentModel.DataAnnotations;
using Sienar.Services;

namespace Sienar.Identity.Requests;

public class PerformEmailChangeRequest : IRequest
{
	[Required]
	public Guid UserId { get; set; }

	[Required]
	public Guid VerificationCode { get; set; }
}