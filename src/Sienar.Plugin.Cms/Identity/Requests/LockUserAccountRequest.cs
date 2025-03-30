using System;
using System.Collections.Generic;
using Sienar.Services;

namespace Sienar.Identity.Requests;

public class LockUserAccountRequest : IRequest
{
	public Guid UserId { get; set; }
	public List<Guid> Reasons { get; set; } = [];
	public DateTime? EndDate { get; set; }
}