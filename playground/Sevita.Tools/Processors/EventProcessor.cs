using System;
using System.Linq;
using System.Linq.Expressions;
using Sevita.Tools.Data;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sevita.Tools.Processors;

public class EventProcessor : IEntityFrameworkFilterProcessor<Event>
{
	/// <inheritdoc />
	public IQueryable<Event> Search(IQueryable<Event> dataset, Filter filter) => throw new NotImplementedException();

	/// <inheritdoc />
	public Expression<Func<Event, object>> GetSortPredicate(string? sortName) => throw new NotImplementedException();
}