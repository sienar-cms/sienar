using System;

namespace Sienar.Identity.Requests;

public class PerformLoginRequest
{
	public Guid LoginToken { get; }
	public PerformLoginRequest(Guid loginToken) => LoginToken = loginToken;
}