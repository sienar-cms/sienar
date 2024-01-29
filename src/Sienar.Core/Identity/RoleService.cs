using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Infrastructure;

namespace Sienar.Identity;

public class RoleService : IRoleService
{
	private readonly IDbContextAccessor<DbContext> _contextAccessor;

	public RoleService(IDbContextAccessor<DbContext> contextAccessor)
	{
		_contextAccessor = contextAccessor;
	}

	/// <inheritdoc />
	public async Task<IEnumerable<SienarRole>> Get()
		=> await _contextAccessor.Context
			.Set<SienarRole>()
			.ToListAsync();
}