using System;
using System.Linq;
using System.Linq.Expressions;
using Sevita.Tools.Data;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sevita.Tools.Processors;

public class PromptProcessor : IEntityFrameworkFilterProcessor<Prompt>
{
	/// <inheritdoc />
	public IQueryable<Prompt> Search(IQueryable<Prompt> dataset, Filter filter) => throw new NotImplementedException();

	/// <inheritdoc />
	public Expression<Func<Prompt, object>> GetSortPredicate(string? sortName) => throw new NotImplementedException();
}