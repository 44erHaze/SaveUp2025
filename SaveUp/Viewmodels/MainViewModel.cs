using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SaveUp.Models;
using SaveUp.Services;
using System.ComponentModel;
using Microsoft.Maui.Controls;

namespace SaveUp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string description;
        private decimal price;
        private ObservableCollection<SavedItem> savedItems;

        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public decimal Price
        {
            get => price;
            set
            {
                price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        public ObservableCollection<SavedItem> SavedItems
        {
            get => savedItems;
            set
            {
                savedItems = value;
                OnPropertyChanged(nameof(SavedItems));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand DeleteCommand { get; }

        public MainViewModel()
        {
            SavedItems = new ObservableCollection<SavedItem>();
            SaveCommand = new Command(OnSave);
            ClearCommand = new Command(OnClear);
            DeleteCommand = new Command<SavedItem>(OnDelete);

            // Load persisted data
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var dataService = new DataService();
            var loadedItems = await dataService.LoadDataAsync();
            foreach (var item in loadedItems)
            {
                SavedItems.Add(item);
            }
        }

        private async void OnSave()
        {
            if (!string.IsNullOrWhiteSpace(Description) && Price > 0)
            {
                var newItem = new SavedItem
                {
                    Description = Description,
                    Price = Price,
                    Date = DateTime.Now // Timestamp added
                };
                SavedItems.Add(newItem);
                Description = string.Empty;
                Price = 0;

                var dataService = new DataService();
                await dataService.SaveDataAsync(SavedItems);

                // Send message to show popup
                MessagingCenter.Send(this, "ShowSavedPopup");
            }
        }

        private async void OnClear()
        {
            bool confirmed = await Application.Current.MainPage.DisplayAlert("Löschen", "Möchten Sie alle Artikel löschen?", "Ja", "Nein");
            if (confirmed)
            {
                SavedItems.Clear();
                var dataService = new DataService();
                await dataService.SaveDataAsync(SavedItems);
            }
        }

        private async void OnDelete(SavedItem item)
        {
            bool confirmed = await Application.Current.MainPage.DisplayAlert("Löschen", "Möchten Sie dieses Produkt löschen?", "Ja", "Nein");
            if (confirmed)
            {
                SavedItems.Remove(item);
                var dataService = new DataService();
                await dataService.SaveDataAsync(SavedItems);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
