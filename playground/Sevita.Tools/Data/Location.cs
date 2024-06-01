using System.ComponentModel.DataAnnotations;
using Sienar.Infrastructure.Data;

namespace Sevita.Tools.Data;

public class Location : EntityBase
{
	[Required]
	[MinLength(3)]
	[MaxLength(255)]
	public string Name { get; set; } = string.Empty;

	[Required]
	[MinLength(3)]
	[MaxLength(255)]
	public string Address { get; set; } = string.Empty;

	[Required]
	[MinLength(3)]
	[MaxLength(255)]
	public string City { get; set; } = string.Empty;

	[Required]
	[MinLength(2)]
	[MaxLength(2)]
	public string State { get; set; } = string.Empty;
}