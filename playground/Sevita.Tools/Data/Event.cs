using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sienar.Infrastructure.Data;

namespace Sevita.Tools.Data;

public class Event : EntityBase
{
	public DateTime StartTime { get; set; }

	public DateTime? EndTime { get; set; }

	[Required]
	[MinLength(1)]
	[MaxLength(1000)]
	public string Description { get; set; } = string.Empty;

	public List<Pbs> Individuals { get; set; } = [];
}