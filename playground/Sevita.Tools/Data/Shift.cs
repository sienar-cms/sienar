using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Sienar.Infrastructure.Data;

namespace Sevita.Tools.Data;

/// <summary>
/// Represents a single employee shift worked
/// </summary>
public class Shift : EntityBase
{
	/// <summary>
	/// The time at which the shift started
	/// </summary>
	public DateTime StartTime { get; set; }

	/// <summary>
	/// The time at which the shift ended
	/// </summary>
	/// <remarks>
	/// Internally, Sevita Tools uses this to determine whether a shift is active or not. A <c>null</c> EndTime indicates that the time log is ongoing
	/// </remarks>
	public DateTime? EndTime { get; set; }

	/// <summary>
	/// The time logs generated during the current shift
	/// </summary>
	[NotMapped]
	public List<TimeLog> TimeLogs { get; set; } = [];

	/// <summary>
	/// The events created during the current shift
	/// </summary>
	[NotMapped]
	public List<Event> Events { get; set; } = [];

	/// <summary>
	/// The prompts issued during the current shift
	/// </summary>
	[NotMapped]
	public List<Prompt> Prompts { get; set; } = [];
}