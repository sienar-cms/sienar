using System.Threading.Tasks;
using MudBlazor;
using Sienar.UI;

namespace Sienar.Extensions;

public static class DialogServiceExtensions
{
	public static async Task<bool> ShowConfirmationDialog(
		this IDialogService self,
		string title,
		string question,
		string confirmText = "yes",
		string cancelText = "no",
		Color mainColor = Color.Primary,
		Color cancelColor = Color.Secondary)
	{
		var parameters = new DialogParameters<ConfirmationDialog>
		{
			{ d => d.Title, title },
			{ d => d.Question, question },
			{ d => d.ConfirmText, confirmText },
			{ d => d.CancelText, cancelText },
			{ d => d.MainColor, mainColor },
			{ d => d.CancelColor, cancelColor }
		};

		var dialog = await self.ShowAsync<ConfirmationDialog>(string.Empty, parameters);

		var result = await dialog.Result;
		return !result.Canceled;
	}
}