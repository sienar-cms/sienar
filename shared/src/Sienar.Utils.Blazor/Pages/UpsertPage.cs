using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Sienar.Data;
using Sienar.Extensions;

namespace Sienar.Pages;

/// <summary>
/// A component that provides form submission services for creating and updating entities
/// </summary>
/// <typeparam name="TModel">The type of the form model</typeparam>
public abstract class UpsertPage<TModel> : FormPage<TModel>
	where TModel : new()
{
	/// <summary>
	/// The ID of the entity to edit, if editing
	/// </summary>
	[Parameter]
	public int? Id { get; set; }

	/// <summary>
	/// The entity reader for the current <c>TModel</c>
	/// </summary>
	[Inject]
	protected IEntityReader<TModel> Reader { get; set; } = null!;

	/// <summary>
	/// The entity writer for the current <c>TModel</c>
	/// </summary>
	[Inject]
	protected IEntityWriter<TModel> Writer { get; set; } = null!;

	/// <summary>
	/// Whether the current page represents an edit page or an insert page
	/// </summary>
	protected bool IsEditing => Id.HasValue;

	/// <summary>
	/// The title of the form
	/// </summary>
	protected string Title => IsEditing
		? $"Edit {GetName()}"
		: $"Add {typeof(TModel).GetEntityName()}";

	/// <summary>
	/// The submit button text of the form's submit button
	/// </summary>
	protected string SubmitText => IsEditing
		? $"Update {GetName()}"
		: $"Add {typeof(TModel).GetEntityName()}";

	/// <inheritdoc />
	protected override async Task OnInitializedAsync()
	{
		if (IsEditing)
		{
			await SubmitRequest(
				async () => Model = (await Reader.Read(Id!.Value)).Result ?? new TModel());
		}
	}

	/// <summary>
	/// Submits the upsert form according to whether the form is an edit form or not
	/// </summary>
	protected async Task OnSubmit()
	{
		if (IsEditing)
		{
			await SubmitRequest(() => Writer.Update(Model));
		}
		else
		{
			await SubmitRequest(() => Writer.Create(Model));
		}

		if (WasSuccessful)
		{
			await OnSuccess();
		}
	}

	/// <summary>
	/// Gets the name identifier of the entity being edited
	/// </summary>
	/// <returns></returns>
	protected abstract string GetName();

	/// <summary>
	/// A method to call when the submission is successful 
	/// </summary>
	protected abstract Task OnSuccess();
}
