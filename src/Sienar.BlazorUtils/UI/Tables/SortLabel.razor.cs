using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Sienar.UI.Tables;

public partial class SortLabel<TItem>
{
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public Expression<Func<TItem, object>> For { get; set; } = default!;

	private string _sortLabel = string.Empty;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		var exception = new ArgumentException($"Expression {For.Body} does not return a property on {typeof(TItem)}");

		var m = For.Body switch
		{
			MemberExpression member => member.Member,
			UnaryExpression unary => (unary.Operand as MemberExpression)?.Member,
			_ => throw exception
		};
		if (m is null)
		{
			throw exception;
		}

		_sortLabel = m.Name;
	}
}