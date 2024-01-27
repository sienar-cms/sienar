using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Services;

namespace Sienar.Pages;

public abstract class UpsertPage<TModel> : FormPage<TModel>
	where TModel : new()
{
	[Parameter]
	public Guid? Id { get; set; }

	[Inject]
	protected IEntityReader<TModel> Reader { get; set; } = default!;

	[Inject]
	protected IEntityWriter<TModel> Writer { get; set; } = default!;

	protected abstract string Name { get; }
	protected bool IsEditing => Id.HasValue;
	protected string Title => IsEditing
		? $"Edit {Name}"
		: $"Add {EntityExtensions.GetEntityName<TModel>()}";
	
	protected string SubmitText => IsEditing
		? $"Update {Name}"
		: $"Add {EntityExtensions.GetEntityName<TModel>()}";

	/// <inheritdoc />
	protected override async Task OnInitializedAsync()
	{
		if (IsEditing)
		{
			await SubmitRequest(
				async () => Model = await Reader.Get(Id!.Value) ?? new());
		}
	}

	protected override async Task OnSubmit()
	{
		if (IsEditing)
		{
			await SubmitRequest(() => Writer.Edit(Model));
		}
		else
		{
			await SubmitRequest(() => Writer.Add(Model));
		}

		if (WasSuccessful)
		{
			await OnSuccess();
		}
	}

	protected abstract Task OnSuccess();
}