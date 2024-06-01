using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Sienar.Infrastructure.Data;

namespace Sevita.Tools.Data;

[Index(nameof(Alias), IsUnique = true)]
public class Site : EntityBase
{
	/// <summary>
	/// The site's program name
	/// </summary>
	[Length(6, maximumLength: 6, ErrorMessage = "Site alias must be exactly 6 characters long")]
	[Required]
	public string Alias { get; set; } = string.Empty;

	/// <summary>
	/// A list of all shifts for the current site
	/// </summary>
	public List<Shift> Shifts { get; set; } = [];

	/// <summary>
	/// A list of all individuals at the current site
	/// </summary>
	public List<Pbs> Individuals { get; set; } = [];
}