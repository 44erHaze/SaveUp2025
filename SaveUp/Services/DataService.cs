using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SaveUp.Models;

namespace SaveUp.Services
{
    public class DataService
    {
        private const string FileName = "savedItems.json";

        public async Task SaveDataAsync(ObservableCollection<SavedItem> items)
        {
            var json = JsonSerializer.Serialize(items);
            var filePath = Path.Combine(FileSystem.AppDataDirectory, FileName);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<ObservableCollection<SavedItem>> LoadDataAsync()
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, FileName);
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                var items = JsonSerializer.Deserialize<ObservableCollection<SavedItem>>(json);
                return items ?? new ObservableCollection<SavedItem>();
            }
            return new ObservableCollection<SavedItem>();
        }
    }
}
