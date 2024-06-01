using System;
using System.Linq;
using System.Linq.Expressions;
using Sevita.Tools.Data;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sevita.Tools.Processors;

public class ShiftProcessor : IEntityFrameworkFilterProcessor<Shift>
{
	/// <inheritdoc />
	public IQueryable<Shift> Search(IQueryable<Shift> dataset, Filter filter) => throw new NotImplementedException();

	/// <inheritdoc />
	public Expression<Func<Shift, object>> GetSortPredicate(string? sortName) => throw new NotImplementedException();
}