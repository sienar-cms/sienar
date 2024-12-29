using System;
using System.Collections.Generic;

namespace Sienar.Identity.Results;

public class AccountLockoutResult
{
	public List<LockoutReason> LockoutReasons { get; set; }
	public DateTime? LockoutEnd { get; set; }
}