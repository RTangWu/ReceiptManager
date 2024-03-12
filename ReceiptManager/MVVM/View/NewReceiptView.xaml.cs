using Microsoft.Maui.Controls;
using ReceiptManager.MVVM.Models;
using ReceiptManager.MVVM.ViewModels;
using Xamarin.Essentials;
using FileResult = Microsoft.Maui.Storage.FileResult;
using Geocoding = Microsoft.Maui.Devices.Sensors.Geocoding;
using Geolocation = Microsoft.Maui.Devices.Sensors.Geolocation;
using GeolocationAccuracy = Microsoft.Maui.Devices.Sensors.GeolocationAccuracy;
using GeolocationRequest = Microsoft.Maui.Devices.Sensors.GeolocationRequest;
using MediaPicker = Microsoft.Maui.Media.MediaPicker;
using Permissions = Microsoft.Maui.ApplicationModel.Permissions;

namespace ReceiptManager.MVVM.View;

public partial class NewReceiptView : ContentPage
{
    public NewReceiptView()
	{
		InitializeComponent();
		
    }

    private async void Cancel_Clicked(object sender, EventArgs e)
    {

        await Navigation.PopAsync();
    }

    private async void AddReceiptClicked(object sender, EventArgs e)
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
            var vm = BindingContext as NewReceiptViewModel;

            var selectedGroup = vm.Groups.FirstOrDefault(x => x.IsSelected);

            if (string.IsNullOrWhiteSpace(vm.StoreName))
            {
                await DisplayAlert("Invalid Shop Name", "Please enter a Shop Name.", "Ok");
                return;
            }

           

            if (NumberValidation.IsNotValid)
            {
                await DisplayAlert("Invalid Amounts", "Please enter a valid positive number (0.00) for the amounts.", "Ok");
                return;
            }
            if (string.IsNullOrWhiteSpace(vm.Address))
            {
                await DisplayAlert("Invalid Address", "Please enter a Address.", "Ok");
                return;
            }

            if (vm.Image == null)
            {
                await DisplayAlert("Missing Picture", "Please upload a picture of the receipt.", "Ok");
                return;
            }


            if (selectedGroup != null)
            {
                bool confirmed = await DisplayAlert("Confirmation", "Are you sure you want to add this receipt?", "Yes", "No");

                if (confirmed)
                {
                    int newReceiptId = vm.Receipts.Any() ? vm.Receipts.Max(r => r.Id) + 1 : 1;

                    var receipt = new MyReceipt
                    {
                        Id = newReceiptId,
                        StoreName = vm.StoreName,
                        Amounts = vm.Amounts,
                        Address = vm.Address,
                        Image = vm.Image,
                        GroupId = selectedGroup.Id
                    };

                    vm.Receipts.Add(receipt);
                    await Navigation.PopAsync();
                }
            }
            else
            {
                await DisplayAlert("Invalid Selection", "You must select a group", "Ok");
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }

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