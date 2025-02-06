using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

/// <summary>
/// Represents a tab container that renders <see cref="ITab"/> instances
/// </summary>
public interface ITabGroup
{
	/// <summary>
	/// The tabs registered with the tab group
	/// </summary>
	List<ITab> Tabs { get; }

	/// <summary>
	/// Registers a tab with a tab group
	/// </summary>
	/// <param name="tab">The tab to register</param>
	void AddTab(ITab tab);

	/// <summary>
	/// De-registers a tab with a tab group
	/// </summary>
	/// <param name="tab">The tab to de-register</param>
	void RemoveTab(ITab tab);
}