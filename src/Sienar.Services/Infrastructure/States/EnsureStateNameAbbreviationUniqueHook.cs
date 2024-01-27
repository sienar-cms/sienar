using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Errors;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Services;

namespace Sienar.Infrastructure.States;

public class EnsureStateNameAbbreviationUniqueHook : DbService<State>, IEntityStateValidator<State>
{
	/// <inheritdoc />
	public EnsureStateNameAbbreviationUniqueHook(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<DbService<State, DbContext>> logger,
		INotificationService notifier)
		: base(contextAccessor, logger, notifier) {}

	/// <inheritdoc />
	public async Task<bool> IsValid(State entity, bool adding)
	{
		var valid = true;
		var existing = await EntitySet.FirstOrDefaultAsync(
			s => s.Id != entity.Id && s.Name == entity.Name);
		if (existing is not null)
		{
			Notifier.Error(ErrorMessages.States.DuplicateName);
			valid = false;
		}

		existing = await EntitySet.FirstOrDefaultAsync(s => s.Id != entity.Id && s.Abbreviation == entity.Abbreviation);
		if (existing is not null)
		{
			Notifier.Error(ErrorMessages.States.DuplicateAbbreviation);
			valid = false;
		}

		return valid;
	}
}