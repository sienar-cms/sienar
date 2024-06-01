using System;
using System.Collections.Generic;
using Sienar.Infrastructure.Data;

namespace Sevita.Tools.Data;

/// <summary>
/// Represents a REM time log entry. Events are of a specific type, such as alone time or community outing.
/// </summary>
public class TimeLog : EntityBase
{
	/// <summary>
	/// The time at which the time log event started
	/// </summary>
	public DateTime StartTime { get; set; }

	/// <summary>
	/// The time at which the time log event ended
	/// </summary>
	/// <remarks>
	/// Internally, Sevita Tools uses this to determine whether a time log is active or not. A <c>null</c> EndTime indicates that the time log is ongoing
	/// </remarks>
	public DateTime? EndTime { get; set; }

	/// <summary>
	/// The type of event the time log represents
	/// </summary>
	public TimeLogType Type { get; set; }

	/// <summary>
	/// A list of locations involved in the time log event
	/// </summary>
	public List<Location> Locations { get; set; } = [];

	/// <summary>
	/// A list of persons being served participating in the time log event
	/// </summary>
	public List<Pbs> Individuals { get; set; } = [];
}