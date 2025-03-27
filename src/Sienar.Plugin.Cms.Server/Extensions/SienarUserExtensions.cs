using System;
using Sienar.Configuration;
using Sienar.Identity;

namespace Sienar.Extensions;

public static class SienarUserExtensions
{
	public static bool ShouldSendEmailConfirmationEmail(
		LoginOptions loginOptions,
		SienarOptions sienarOptions)
		=> loginOptions.RequireConfirmedAccount && sienarOptions.EnableEmail;

	public static bool IsLockedOut(this SienarUser user)
		=> user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow;
}