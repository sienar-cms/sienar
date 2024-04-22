#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class SienarUserFilterProcessor : IFilterProcessor<SienarUser>
{
	public IQueryable<SienarUser> Search(IQueryable<SienarUser> dataset, Filter filter)
	{
		if (string.IsNullOrEmpty(filter.SearchTerm))
		{
			return dataset;
		}

		return dataset.Where(
			s => s.Username.Contains(filter.SearchTerm)
			|| s.Email.Contains(filter.SearchTerm)
			|| !string.IsNullOrEmpty(s.PendingEmail) && s.PendingEmail.Contains(filter.SearchTerm));
	}

	public IQueryable<SienarUser> ProcessIncludes(IQueryable<SienarUser> dataset, Filter filter)
	{
		if (filter.Includes.Count == 0)
		{
			return dataset;
		}

		if (filter.Includes.Contains(nameof(SienarUser.Roles)))
		{
			dataset = dataset.Include(u => u.Roles);
		}

		return dataset;
	}

	public Expression<Func<SienarUser, object>> GetSortPredicate(string? sortName) => sortName switch
	{
		nameof(SienarUser.Username) => u => u.Username,
		nameof(SienarUser.Email) => u => u.Email,
		nameof(SienarUser.PendingEmail) => u => u.PendingEmail!,
		_ => u => u.Username
	};

	public Filter ModifyFilter(Filter? filter, ActionType action)
	{
		if (filter is null) return new Filter { Includes = ["Roles"] };
		if (!filter.Includes.Contains("Roles")) filter.Includes.Add("Roles");
		return filter;
	}
}