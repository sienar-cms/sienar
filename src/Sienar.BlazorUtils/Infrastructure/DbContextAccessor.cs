using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Sienar.Infrastructure;

public class DbContextAccessor<TContext> : IDbContextAccessor<TContext>
	where TContext : DbContext
{
	private readonly IServiceProvider _sp;

	public DbContextAccessor(IServiceProvider sp)
	{
		_sp = sp;
		Context = sp.GetRequiredService<TContext>();
	}

	public TContext Context { get; private set; }

	/// <inheritdoc />
	public void RefreshContext()
	{
		try
		{
			var old = Context;
			Context = _sp.GetRequiredService<TContext>();
			old.Dispose();
		}
		catch (ObjectDisposedException) {}
	}
}