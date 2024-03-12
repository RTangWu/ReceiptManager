using Microsoft.Maui.Controls;
using ReceiptManager.MVVM.Models;
using ReceiptManager.MVVM.ViewModels;



namespace ReceiptManager.MVVM.View;

public partial class AllReceiptsView : ContentPage
{
	private AllReceiptsViewModel allReceiptviewModel = new AllReceiptsViewModel();
    public AllReceiptsView()
	{
		InitializeComponent();
        allReceiptviewModel = new AllReceiptsViewModel();
		BindingContext = allReceiptviewModel;


    }

	private void checkBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		allReceiptviewModel.UpdateData();

        
    }

    private void OnReceiptChangesSaved(object sender, EventArgs e)
    {
        // Update the data and refresh the view when changes are saved
        allReceiptviewModel.UpdateData();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
		var receiptView = new NewReceiptView
		{
			BindingContext = new NewReceiptViewModel
			{
				Receipts = allReceiptviewModel.Receipts,
				Groups = allReceiptviewModel.Groups,

			}
		};

		Navigation.PushAsync(receiptView);
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {

        try
        {
            // Check for network connectivity
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                // No internet connection, display message
                await DisplayAlert("No Internet", "Please check your internet connection and try again.", "OK");
                return;
            }
            var swipeItem = sender as SwipeItem;
            var receipt = swipeItem?.BindingContext as MyReceipt;

            if (receipt != null)
            {
                bool confirmed = await DisplayAlert("Confirmation", "Are you sure you want to delete this receipt?", "Yes", "No");

                if (confirmed)
                {
                    allReceiptviewModel.Receipts.Remove(receipt);

                    var lastItemIndex = allReceiptviewModel.Receipts.Count - 1;
                    if (lastItemIndex >= 0)
                    {
                        ReceiptsCollectionView.ScrollTo(lastItemIndex, position: ScrollToPosition.MakeVisible, animate: false);
                    }

                    allReceiptviewModel.UpdateData();
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }

    }

    private async void EditButton_Clicked(object sender, EventArgs e)
    {
        var swipeItem = sender as SwipeItem;
        var selectedReceipt = swipeItem?.BindingContext as MyReceipt;

        if (selectedReceipt != null)
        {
            var editReceiptView = new EditReceiptView
            {
                BindingContext = new EditReceiptViewModel
                {
                    Id = selectedReceipt.Id,
                    StoreName = selectedReceipt.StoreName,
                    Amounts = selectedReceipt.Amounts,
                    Address = selectedReceipt.Address,
                    Image = selectedReceipt.Image,
                    Receipts = allReceiptviewModel.Receipts,
                    Groups = allReceiptviewModel.Groups
                }
            };
            (editReceiptView.BindingContext as EditReceiptViewModel).ChangesSaved += OnReceiptChangesSaved;

            await Navigation.PushAsync(editReceiptView);
        }
    }

    private async void ReceiptItemClicked(object sender, EventArgs e)
    {
        var frame = sender as Frame;
        var selectedReceipt = frame?.BindingContext as MyReceipt;

        if (selectedReceipt != null)
        {
            var editReceiptView = new EditReceiptView
            {
                BindingContext = new EditReceiptViewModel
                {
                    Id = selectedReceipt.Id,
                    StoreName = selectedReceipt.StoreName,
                    Amounts = selectedReceipt.Amounts,
                    Address= selectedReceipt.Address,
                    Image = selectedReceipt.Image,
                    Receipts = allReceiptviewModel.Receipts,
                    Groups = allReceiptviewModel.Groups
                }
            };
            (editReceiptView.BindingContext as EditReceiptViewModel).ChangesSaved += OnReceiptChangesSaved;

            await Navigation.PushAsync(editReceiptView);
        }
    }





}