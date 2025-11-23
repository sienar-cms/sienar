using Sienar.Services;

namespace Sienar.Identity.Requests;

public class ManuallyConfirmUserAccountRequest : IRequest
{
	public int UserId { get; set; }
}