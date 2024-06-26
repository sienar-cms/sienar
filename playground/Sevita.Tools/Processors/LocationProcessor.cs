using System;
using System.Linq;
using System.Linq.Expressions;
using Sevita.Tools.Data;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sevita.Tools.Processors;

public class LocationProcessor : IEntityFrameworkFilterProcessor<Location>
{
	/// <inheritdoc />
	public IQueryable<Location> Search(IQueryable<Location> dataset, Filter filter) => throw new NotImplementedException();

	/// <inheritdoc />
	public Expression<Func<Location, object>> GetSortPredicate(string? sortName) => throw new NotImplementedException();
}