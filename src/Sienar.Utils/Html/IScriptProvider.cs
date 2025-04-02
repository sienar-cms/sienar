using System.Collections.Generic;

namespace Sienar.Html;

/// <summary>
/// A container for <see cref="ScriptResource"/> instances that should be rendered on each page
/// </summary>
public interface IScriptProvider : IList<ScriptResource>;