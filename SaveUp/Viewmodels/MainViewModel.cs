using System;
using System.Collections.ObjectModel;
using System.Linq;
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
        public ICommand SortCommand { get; }

        public MainViewModel()
        {
            SavedItems = new ObservableCollection<SavedItem>();
            SaveCommand = new Command(OnSave);
            ClearCommand = new Command(OnClear);
            DeleteCommand = new Command<SavedItem>(OnDelete);
            SortCommand = new Command<int>(OnSort);  // Change here

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

        // Add a new property for SelectedSortIndex
        private int selectedSortIndex;

        public int SelectedSortIndex
        {
            get => selectedSortIndex;
            set
            {
                if (selectedSortIndex != value)
                {
                    selectedSortIndex = value;
                    OnPropertyChanged(nameof(SelectedSortIndex));
                    OnSort(selectedSortIndex); // Update sorting logic
                }
            }
        }

        private void OnSort(int selectedSortIndex)
        {
            switch (selectedSortIndex)
            {
                case 0: // Preis aufsteigend
                    SavedItems = new ObservableCollection<SavedItem>(SavedItems.OrderBy(item => item.Price));
                    break;
                case 1: // Preis absteigend
                    SavedItems = new ObservableCollection<SavedItem>(SavedItems.OrderByDescending(item => item.Price));
                    break;
                case 2: // Alphabetisch aufsteigend
                    SavedItems = new ObservableCollection<SavedItem>(SavedItems.OrderBy(item => item.Description));
                    break;
                case 3: // Alphabetisch absteigend
                    SavedItems = new ObservableCollection<SavedItem>(SavedItems.OrderByDescending(item => item.Description));
                    break;
                case 4: // Datum aufsteigend
                    SavedItems = new ObservableCollection<SavedItem>(SavedItems.OrderBy(item => item.Date));
                    break;
                case 5: // Datum absteigend
                    SavedItems = new ObservableCollection<SavedItem>(SavedItems.OrderByDescending(item => item.Date));
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
