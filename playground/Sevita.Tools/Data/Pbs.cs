using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sienar.Infrastructure.Data;

namespace Sevita.Tools.Data;

public class Pbs : EntityBase
{
	[Required]
	[MinLength(2)]
	[MaxLength(5)]
	public string Initials { get; set; } = string.Empty;

	public bool IsHab { get; set; }

	public List<Goal> Goals { get; set; } = [];
}