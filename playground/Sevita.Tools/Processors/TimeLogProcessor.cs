using System;
using System.Linq;
using System.Linq.Expressions;
using Sevita.Tools.Data;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sevita.Tools.Processors;

public class TimeLogProcessor : IEntityFrameworkFilterProcessor<TimeLog>
{
	/// <inheritdoc />
	public IQueryable<TimeLog> Search(IQueryable<TimeLog> dataset, Filter filter) => throw new NotImplementedException();

	/// <inheritdoc />
	public Expression<Func<TimeLog, object>> GetSortPredicate(string? sortName) => throw new NotImplementedException();
}