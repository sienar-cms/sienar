using System;

namespace Sienar.Identity.Requests;

public class ManuallyConfirmUserAccountRequest
{
	public Guid UserId { get; set; }
}