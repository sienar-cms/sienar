using System;
using System.Collections.Generic;
using Sienar.Services;

namespace Sienar.Identity.Requests;

public class LockUserAccountRequest : IRequest
{
	public int UserId { get; set; }
	public List<int> Reasons { get; set; } = [];
	public DateTime? EndDate { get; set; }
}