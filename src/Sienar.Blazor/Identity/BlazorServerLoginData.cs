using System;

namespace Sienar.Identity;

public class BlazorServerLoginData
{
	public required Guid UserId { get; set; }
	public required DateTimeOffset ExpiresAt { get; set; }
	public required bool IsPersistent { get; set; }
	public required bool IsAuthenticated { get; set; }
}