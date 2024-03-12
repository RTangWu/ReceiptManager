using ReceiptManager.MVVM.ViewModels;
using ReceiptManager.MVVM.Models;
using System.Globalization;
namespace ReceiptManager.MVVM.View;

public partial class EditReceiptView : ContentPage
{
   
    public EditReceiptView()
	{
		InitializeComponent();
      

    }


    private async void SaveButton_Clicked(object sender, EventArgs e)
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
            var vm = BindingContext as EditReceiptViewModel;

            var selectedGroup = vm.Groups.FirstOrDefault(x => x.IsSelected);

            if (string.IsNullOrWhiteSpace(vm.StoreName))
            {
                await DisplayAlert("Invalid Shop Name", "Please enter a Shop Name.", "Ok");
                return;
            }
            if (string.IsNullOrWhiteSpace(vm.Address))
            {
                await DisplayAlert("Invalid Address", "Please enter a Address.", "Ok");
                return;
            }

            if (NumberValidation.IsNotValid)
            {
                await DisplayAlert("Invalid Amounts", "Please enter a valid positive number (0.00) for the amounts.", "Ok");
                return;
            }







            if (selectedGroup == null)
            {
                await DisplayAlert("Invalid Selection", "You must select a group", "Ok");
                return;
            }
            var existingReceipt = vm.Receipts.FirstOrDefault(r => r.Id == vm.Id);

            if (existingReceipt != null)
            {
                bool proceed = await DisplayAlert("Confirmation", "Are You sure you want to save change?", "Yes", "No");
                if (!proceed)
                    return;
                existingReceipt.StoreName = vm.StoreName;
                existingReceipt.Amounts = vm.Amounts;
                existingReceipt.Address = vm.Address;
                existingReceipt.Image = vm.Image;
                existingReceipt.GroupId = selectedGroup.Id;

                vm.SaveChanges();
                await Navigation.PopAsync();
            }
            else
            {
                // If the selected receipt doesn't exist, display an error message
                await DisplayAlert("Error", "The selected receipt does not exist.", "Ok");
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }

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
            var vm = BindingContext as EditReceiptViewModel;

            // Get the selected receipt to delete
            var selectedReceipt = vm.Receipts.FirstOrDefault(r => r.Id == vm.Id);

            if (selectedReceipt != null)
            {
                bool proceed = await DisplayAlert("Confirmation", "Are you sure you want to delete this receipt?", "Yes", "No");
                if (proceed)
                {
                    // Remove the selected receipt from the collection
                    vm.Receipts.Remove(selectedReceipt);

                    // Optionally, save changes to persist the deletion
                    vm.SaveChanges();

                    // Navigate back to the previous page
                    await Navigation.PopAsync();
                }
            }
            else
            {
                // If the selected receipt doesn't exist, display an error message
                await DisplayAlert("Error", "The selected receipt does not exist.", "Ok");
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void Cancel_Clicked(object sender, EventArgs e)
    {

        await Navigation.PopAsync();
    }

    private async void FindAddressClicked(object sender, EventArgs e)
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

            // Show loading indicator
            activityIndicator.IsRunning = true;
            activityIndicator.IsVisible = true;

            // Request permission to access location
            var status = await Permissions.RequestAsync<Microsoft.Maui.ApplicationModel.Permissions.LocationWhenInUse>();

            if (status != Microsoft.Maui.ApplicationModel.PermissionStatus.Granted)
            {
                // Permission denied, hide loading indicator and display message or handle accordingly
                activityIndicator.IsRunning = false;
                activityIndicator.IsVisible = false;
                await DisplayAlert("Permission Denied", "Location permission is required to retrieve your current location.", "OK");
                return;
            }

            // Get the current location
            var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));

            if (location != null)
            {
                // Perform reverse geocoding to get address from coordinates
                var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);

                if (placemarks != null && placemarks.Any())
                {
                    // Extract the address components from the placemark
                    var placemark = placemarks.FirstOrDefault();
                    string address = $"{placemark.SubThoroughfare} {placemark.Thoroughfare}";

                    // Update the address entry textbox with the location information
                    if (FindByName("EntryAddress") is Entry addressEntry)
                    {
                        addressEntry.Text = address;
                    }
                }
                else
                {
                    // No address found, display message or handle accordingly
                    await DisplayAlert("Address Unavailable", "Unable to retrieve address for your current location.", "OK");
                }
            }
            else
            {
                // Location not available, display message or handle accordingly
                await DisplayAlert("Location Unavailable", "Unable to retrieve your current location.", "OK");
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
        finally
        {
            // Hide loading indicator
            activityIndicator.IsRunning = false;
            activityIndicator.IsVisible = false;
        }
    }

    private async void UploadImageClicked(object sender, EventArgs e)
    {
        // Show the loading indicator
        activityIndicator.IsVisible = true;
        activityIndicator.IsRunning = true;

        try
        {
            // Check for network connectivity
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                // No internet connection, display message
                await DisplayAlert("No Internet", "Please check your internet connection and try again.", "OK");
                return;
            }

            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    // Convert the captured photo to a byte array
                    byte[] imageData;
                    using (Stream stream = await photo.OpenReadAsync())
                    {
                        MemoryStream ms = new MemoryStream();
                        await stream.CopyToAsync(ms);
                        imageData = ms.ToArray();
                    }

                    // Convert the byte array to an ImageSource
                    ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(imageData));

                    // Update the Image property in the view model
                    Picture.Source = imageSource;

                    // Show confirmation message
                    await DisplayAlert("Success", "Photo captured and updated successfully.", "OK");
                }
                else
                {
                    // Show error message if photo is null
                    await DisplayAlert("Error", "Failed to capture photo.", "OK");
                }
            }
            else
            {
                // Show error message if capture is not supported
                await DisplayAlert("Error", "Photo capture is not supported on this device.", "OK");
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
            await DisplayAlert("Permission Denied", $"An error occurred: {ex.Message}", "OK");
        }
        finally
        {
            // Hide the loading indicator
            activityIndicator.IsVisible = false;
            activityIndicator.IsRunning = false;
        }
    }


}