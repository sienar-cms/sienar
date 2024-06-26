using System;
using System.ComponentModel.DataAnnotations;
using Sienar.Infrastructure.Data;

namespace Sevita.Tools.Data;

public class Objective : EntityBase
{
	[Required]
	[MinLength(1)]
	[MaxLength(1000)]
	public string Text { get; set; } = string.Empty;

	public DateTime StartDate { get; set; }

	public DateTime EndDate { get; set; }
}