using System;
using System.Collections.Generic;

namespace Sienar.Infrastructure.Entities;

public class PagedQuery<TModel>
{
	/// <summary>
	/// Represents the items in the current page of results
	/// </summary>
	public IEnumerable<TModel> Items { get; set; } = Array.Empty<TModel>();

	/// <summary>
	/// Represents the total number of items across all pages of results
	/// </summary>
	public int TotalCount { get; set; }

	public PagedQuery() {}

	public PagedQuery(IEnumerable<TModel> items, int totalCount)
	{
		Items = items;
		TotalCount = totalCount;
	}
}