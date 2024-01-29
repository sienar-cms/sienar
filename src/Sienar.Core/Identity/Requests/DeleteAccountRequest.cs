using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sienar.Identity.Requests;

public class DeleteAccountRequest
{
	[Required]
	[DisplayName("Password")]
	[DataType(DataType.Password)]
	public string Password { get; set; } = default!;
}