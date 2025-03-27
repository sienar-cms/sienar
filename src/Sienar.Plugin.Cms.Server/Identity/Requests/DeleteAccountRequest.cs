using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sienar.Services;

namespace Sienar.Identity.Requests;

public class DeleteAccountRequest : IRequest
{
	[Required]
	[DisplayName("Password")]
	[DataType(DataType.Password)]
	public string Password { get; set; } = default!;
}