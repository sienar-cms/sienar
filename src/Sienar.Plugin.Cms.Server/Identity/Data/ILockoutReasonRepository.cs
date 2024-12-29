using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sienar.Data;

namespace Sienar.Identity.Data;

/// <summary>
/// Adds additional methods for querying <see cref="LockoutReason"/> database entries
/// </summary>
public interface ILockoutReasonRepository : IRepository<LockoutReason>
{
	/// <summary>
	/// Reads multiple lockout reasons using an enumerable of their IDs
	/// </summary>
	/// <param name="ids">the IDs of lockout reasons to read</param>
	/// <returns>a list of lockout reasons matching the input IDs</returns>
	Task<List<LockoutReason>> Read(List<Guid> ids);
}