using System;
using System.Collections.Generic;

namespace Sienar.Identity.Requests;

public class LockUserAccountRequest
{
	public Guid UserId { get; set; }
	public IEnumerable<Guid> Reasons { get; set; } = [];
	public DateTime EndDate { get; set; }
}