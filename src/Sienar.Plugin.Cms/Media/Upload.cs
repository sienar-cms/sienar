using System;
using System.IO;
using Sienar.Identity;
using Sienar.Data;

namespace Sienar.Media;

[EntityName(Singular = "file", Plural = "files")]
public class Upload : EntityBase
{
	/// <summary>
	/// The title of the image
	/// </summary>
	public string Title { get; set; } = string.Empty;

	/// <summary>
	/// The path to the file
	/// </summary>
	public string Path { get; set; } = string.Empty;

	/// <summary>
	/// The description of the file
	/// </summary>
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

	/// <summary>
	/// The <see cref="SienarUser"/> who uploaded the file
	/// </summary>
	public SienarUser? User { get; set; }

	public bool IsUnassigned => !UserId.HasValue;

	public Stream? Contents { get; set; }
}