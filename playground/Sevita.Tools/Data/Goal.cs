using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sienar.Infrastructure.Data;

namespace Sevita.Tools.Data;

public class Goal : EntityBase
{
	[Required]
	[MinLength(10)]
	[MaxLength(255)]
	public string Text { get; set; } = string.Empty;

	public DateTime StartDate { get; set; }

	public DateTime EndDate { get; set; }

	public List<Objective> Objectives { get; set; } = [];
}