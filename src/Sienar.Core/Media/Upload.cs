using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Sienar.Infrastructure.Entities;

namespace Sienar.Media;

[EntityName(Singular = "file", Plural = "files")]
public class Upload : EntityBase
{
	/// <summary>
	/// The title of the image
	/// </summary>
	[Required]
	[MinLength(1)]
	[MaxLength(100)]
	public string Title { get; set; } = string.Empty;

	/// <summary>
	/// The path to the file
	/// </summary>
	[Required]
	[MinLength(1)]
	[MaxLength(300)]
	public string Path { get; set; } = string.Empty;

	/// <summary>
	/// The description of the file
	/// </summary>
	[MaxLength(300)]
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// The type of the medium
	/// </summary>
	public MediaType MediaType { get; set; }

	/// <summary>
	/// The date and time the medium was created
	/// </summary>
	public DateTime UploadedAt { get; set; }

	/// <summary>
	/// Indicates whether the file should be considered private to the user who uploaded it
	/// </summary>
	public bool IsPrivate { get; set; }

	/// <summary>
	/// The GUID of the user who uploaded the file
	/// </summary>
	public Guid? UserId { get; set; }

	[NotMapped]
	public bool IsUnassigned => !UserId.HasValue;

	[NotMapped]
	public IFormFile? Contents { get; set; }
}