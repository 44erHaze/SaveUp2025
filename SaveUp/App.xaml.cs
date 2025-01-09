using Microsoft.Maui.Controls;
using SaveUp.Views;

namespace SaveUp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            
            MainPage = new NavigationPage(new StartPage());
        }
    }

}
