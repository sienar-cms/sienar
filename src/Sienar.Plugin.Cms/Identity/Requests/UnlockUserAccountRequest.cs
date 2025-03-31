using System;
using Sienar.Services;

namespace Sienar.Identity.Requests;

public class UnlockUserAccountRequest : IRequest
{
	public Guid UserId { get; set; }
}