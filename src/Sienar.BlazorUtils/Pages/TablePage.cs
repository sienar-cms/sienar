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

	protected async Task DeleteEntity(Guid id)
	{
		var entityName = typeof(TEntity).GetEntityName();
		var shouldDelete = await DialogService.ShowConfirmationDialog(
			$"Delete {entityName}",
			$"Are you sure you want to delete this {entityName}? This cannot be undone!");

		if (shouldDelete && await Deleter.Delete(id))
		{
			await Table.ReloadTable();
		}
	}
}