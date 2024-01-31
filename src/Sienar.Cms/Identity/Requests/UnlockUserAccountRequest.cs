using System;

namespace Sienar.Identity.Requests;

public class UnlockUserAccountRequest
{
	public Guid UserId { get; set; }

	public UnlockUserAccountRequest(Guid userId) => UserId = userId;
}