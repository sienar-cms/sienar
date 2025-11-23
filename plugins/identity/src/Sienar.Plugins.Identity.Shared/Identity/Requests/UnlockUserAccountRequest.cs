using Sienar.Services;

namespace Sienar.Identity.Requests;

public class UnlockUserAccountRequest : IRequest
{
	public int UserId { get; set; }
}