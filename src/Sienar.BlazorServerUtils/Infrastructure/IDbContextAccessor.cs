using Microsoft.EntityFrameworkCore;

namespace Sienar.Infrastructure;

// ReSharper disable once TypeParameterCanBeVariant
public interface IDbContextAccessor<TContext>
	where TContext : DbContext
{
	TContext Context { get; }

	void RefreshContext();
}