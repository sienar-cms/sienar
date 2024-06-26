using System;
using Sienar.Infrastructure.Data;

namespace Sevita.Tools.Data;

public class Prompt : EntityBase
{
	public DateTime Time { get; set; }

	public PromptOutcome Outcome { get; set; }

	public Guid ObjectiveId { get; set; }

	public Objective Objective { get; set; } = default!;
}