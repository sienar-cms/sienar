using Microsoft.EntityFrameworkCore;
using Sienar;

namespace Sevita.Tools.Data;

public class AppDbContext : SienarDbContext
{
	/// <inheritdoc />
	public AppDbContext(DbContextOptions options)
		: base(options) {}

	public DbSet<Site> Sites { get; set; }
	public DbSet<Shift> Shifts { get; set; }
	public DbSet<TimeLog> TimeLogs { get; set; }
	public DbSet<Location> Locations { get; set; }
	public DbSet<Pbs> Individuals { get; set; }
	public DbSet<Goal> Goals { get; set; }
	public DbSet<Objective> Objectives { get; set; }
	public DbSet<Event> Events { get; set; }
	public DbSet<Prompt> Prompts { get; set; }
}