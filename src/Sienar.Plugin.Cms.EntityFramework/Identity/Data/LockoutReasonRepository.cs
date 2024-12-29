using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Data;
using Sienar.Processors;

namespace Sienar.Identity.Data;

public class LockoutReasonRepository<TContext> : EntityFrameworkRepository<LockoutReason, TContext>,
	ILockoutReasonRepository
	where TContext : DbContext
{
	/// <inheritdoc />
	public LockoutReasonRepository(
		TContext context,
		IEntityFrameworkFilterProcessor<LockoutReason> filterProcessor)
		: base(context, filterProcessor) {}

	/// <inheritdoc />
	public async Task<List<LockoutReason>> Read(List<Guid> ids)
		=> await Context
			.Set<LockoutReason>()
			.Where(l => ids.Contains(l.Id))
			.ToListAsync();
}