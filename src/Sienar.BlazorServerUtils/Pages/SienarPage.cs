using System;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Sienar.Infrastructure;

namespace Sienar.Pages;

public class SienarPage<TContext> : ComponentBase, IDisposable
	where TContext : DbContext
{
	private bool _disposed;

	[Inject]
	protected IDbContextAccessor<TContext> ContextAccessor { get; set; } = default!;

	/// <inheritdoc />
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (_disposed || !disposing) return;
		ContextAccessor.RefreshContext();
		_disposed = true;
	}
}

public class SienarPage : SienarPage<DbContext>;