using System;
using Microsoft.AspNetCore.Components;

namespace Sienar.Layouts;

public abstract class StandardLayoutBase : LayoutComponentBase
{
	protected string[] MenuNames = Array.Empty<string>();
}