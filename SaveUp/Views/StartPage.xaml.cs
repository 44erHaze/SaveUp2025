using Microsoft.Maui.Controls;

namespace SaveUp.Views
{
    public partial class StartPage : ContentPage
    {
        public StartPage()
        {
            InitializeComponent();
        }

        private async void OnCreateProductClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }

        private async void OnShowListClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListPage());
        }
    }
}
