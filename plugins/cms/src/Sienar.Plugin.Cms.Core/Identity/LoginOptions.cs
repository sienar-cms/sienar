using System;

namespace Sienar.Identity;

public class LoginOptions
{
	/// <summary>
	/// The duration of a persistent login (Remember Me enabled), in days
	/// </summary>
	public double PersistentLoginDuration { get; set; } = 30;

	/// <summary>
	/// The duration of a transient login (Remember Me NOT enabled), in hours
	/// </summary>
	public double TransientLoginDuration { get; set; } = 2;

	/// <summary>
	/// Whether to require users to confirm their account prior to login
	/// </summary>
	public bool RequireConfirmedAccount { get; set; } = true;

	/// <summary>
	/// How long a user should be locked out following multiple failed login attempts
	/// </summary>
	public TimeSpan LockoutTimespan { get; set; } = TimeSpan.FromMinutes(15);

	/// <summary>
	/// The number of login attempts to allow prior to lockout. E.g., if value is "3", then lockout will occur on the 3rd failure
	/// </summary>
	public int MaxFailedLoginAttempts { get; set; } = 3;
}