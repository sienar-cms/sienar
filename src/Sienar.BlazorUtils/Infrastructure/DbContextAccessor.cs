using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Sienar.Infrastructure;

public class DbContextAccessor<TContext> : IDbContextAccessor<TContext>
	where TContext : DbContext
{
	protected readonly IServiceProvider ServiceProvider;

	public DbContextAccessor(IServiceProvider sp)
	{
		ServiceProvider = sp;
		Context = sp.GetRequiredService<TContext>();
	}

	public TContext Context { get; private set; }

	/// <inheritdoc />
	public void RefreshContext()
	{
		try
		{
			var old = Context;
			Context = ServiceProvider.GetRequiredService<TContext>();
			old.Dispose();
		}
		catch (ObjectDisposedException) {}
	}
}