using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sienar.Infrastructure.Entities;

namespace Sienar.Identity;

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

	/// <inheritdoc />
	public IQueryable<SienarUser> ProcessIncludes(IQueryable<SienarUser> dataset, Filter filter)
	{
		if (filter.Includes is null || !filter.Includes.Any())
		{
			return dataset;
		}

		if (filter.Includes.Contains(nameof(SienarUser.Roles)))
		{
			dataset = dataset.Include(u => u.Roles);
		}

		return dataset;
	}

	/// <inheritdoc />
	public Expression<Func<SienarUser, object>> GetSortPredicate(string? sortName) => sortName switch
	{
		nameof(SienarUser.Username) => u => u.Username,
		nameof(SienarUser.Email) => u => u.Email,
		nameof(SienarUser.PendingEmail) => u => u.PendingEmail!,
		_ => u => u.Username
	};
}