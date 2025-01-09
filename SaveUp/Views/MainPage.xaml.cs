using Microsoft.Maui.Controls;
using SaveUp.ViewModels;

namespace SaveUp.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            // Subscribe to the "ShowSavedPopup" message
            MessagingCenter.Subscribe<MainViewModel>(this, "ShowSavedPopup", async (sender) =>
            {
                await DisplayAlert("Gespart", "Du hast etwas gespart!", "OK");
            });
        }

        private async void OnShowListClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListPage());
        }
    }
}
