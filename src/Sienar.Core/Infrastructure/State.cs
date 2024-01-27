using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sienar.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Sienar.Infrastructure;

[EntityName(Singular = "state", Plural = "states")]
[Index(nameof(Name), IsUnique = true)]
[Index(nameof(Abbreviation), IsUnique = true)]
public class State : EntityBase
{
	[Required]
	[DisplayName("Name")]
	[MaxLength(20, ErrorMessage = "The state name must be no longer than 20 characters long")]
	public string Name { get; set; } = string.Empty;

	[Required]
	[DisplayName("Abbreviation")]
	[MaxLength(2, ErrorMessage = "The state abbreviation must be exactly 2 characters long")]
	[MinLength(2, ErrorMessage = "The state abbreviation must be exactly 2 characters long")]
	public string Abbreviation { get; set; } = string.Empty;
}