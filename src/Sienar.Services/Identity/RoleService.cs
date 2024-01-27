using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Infrastructure;

namespace Sienar.Identity;

public class RoleService : IRoleService
{
	protected readonly IDbContextAccessor<DbContext> ContextAccessor;

	public RoleService(IDbContextAccessor<DbContext> contextAccessor)
	{
		ContextAccessor = contextAccessor;
	}

	/// <inheritdoc />
	public async Task<IEnumerable<SienarRole>> Get()
		=> await ContextAccessor.Context
			.Set<SienarRole>()
			.ToListAsync();
}