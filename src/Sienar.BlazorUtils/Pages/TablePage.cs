using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Services;
using Sienar.UI.Tables;

namespace Sienar.Pages;

public class TablePage<TEntity> : SienarPage
	where TEntity : EntityBase
{
	[Inject]
	protected IEntityReader<TEntity> Reader { get; set; } = default!;

	[Inject]
	protected IEntityDeleter<TEntity> Deleter { get; set; } = default!;

	[Inject]
	private IDialogService DialogService { get; set; } = default!;

	protected TemplatedTable<TEntity> Table = default!;

	protected Task DeleteEntity(
		Guid id,
		string? title = null,
		string? question = null)
	{
		var entityName = typeof(TEntity).GetEntityName();
		title ??= $"Delete {entityName}";
		question ??= $"Are you sure you want to delete this {entityName}? This cannot be undone!";

		return ConfirmAction(
			title,
			question,
			() => Deleter.Delete(id),
			mainColor: Color.Error,
			cancelColor: Color.Primary);
	}

	protected async Task ConfirmAction(
		string title,
		string question,
		Func<Task<bool>> action,
		string confirmText = "Yes",
		string cancelText = "no",
		Color mainColor = Color.Primary,
		Color cancelColor = Color.Secondary)
	{
		var shouldAct = await DialogService.ShowConfirmationDialog(
			title,
			question,
			confirmText,
			cancelText,
			mainColor,
			cancelColor);

		if (shouldAct && await action())
		{
			await Table.ReloadTable();
		}
	}
}