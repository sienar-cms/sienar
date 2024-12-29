using System;
using System.Linq;
using System.Linq.Expressions;
using Sienar.Data;
using Sienar.Processors;

namespace Sienar.Media.Processors;

public class UploadFilterProcessor : IEntityFrameworkFilterProcessor<Upload>
{
	/// <inheritdoc />
	public IQueryable<Upload> Search(IQueryable<Upload> dataset, Filter filter)
	{
		if (string.IsNullOrEmpty(filter.SearchTerm))
		{
			return dataset;
		}

		return dataset.Where(
			m => m.Title.Contains(filter.SearchTerm)
			|| m.Path.Contains(filter.SearchTerm)
			|| m.Description.Contains(filter.SearchTerm));
	}

	/// <inheritdoc />
	public Expression<Func<Upload, object>> GetSortPredicate(string? sortName) => sortName switch
	{
		nameof(Upload.UploadedAt) => m => m.UploadedAt,
		nameof(Upload.MediaType) => m => m.MediaType,
		_ => m => m.Title
	};
}