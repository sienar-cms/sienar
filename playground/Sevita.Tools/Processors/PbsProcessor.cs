using System;
using System.Linq;
using System.Linq.Expressions;
using Sevita.Tools.Data;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sevita.Tools.Processors;

public class PbsProcessor : IEntityFrameworkFilterProcessor<Pbs>
{
	/// <inheritdoc />
	public IQueryable<Pbs> Search(IQueryable<Pbs> dataset, Filter filter) => throw new NotImplementedException();

	/// <inheritdoc />
	public Expression<Func<Pbs, object>> GetSortPredicate(string? sortName) => throw new NotImplementedException();
}