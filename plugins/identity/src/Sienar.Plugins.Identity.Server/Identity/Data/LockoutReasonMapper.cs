using Sienar.Data;
using Sienar.Extensions;

namespace Sienar.Identity.Data;

/// <summary>
/// Maps between <see cref="LockoutReasonDto"/> and <see cref="LockoutReason"/>
/// </summary>
public class LockoutReasonMapper : IMapper<LockoutReasonDto, LockoutReason>
{
	/// <inheritdoc />
	public void MapToEntity(LockoutReasonDto dto, LockoutReason entity)
	{
		entity.Reason = dto.Reason;
		entity.NormalizedReason = dto.Reason.ToNormalized();
	}

	/// <inheritdoc />
	public void MapToDto(LockoutReasonDto dto, LockoutReason entity)
	{
		dto.Id = entity.Id;
		dto.ConcurrencyStamp = entity.ConcurrencyStamp;
		dto.Reason = entity.Reason;
	}
}
