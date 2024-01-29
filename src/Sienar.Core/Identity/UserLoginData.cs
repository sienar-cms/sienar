using System;

namespace Sienar.Identity;

public class UserLoginData
{
	public Guid? Id { get; set; }
	public DateTime LoginExpiresAt { get; set; } = DateTime.MinValue;
	public bool IsPersistent { get; set; }
}