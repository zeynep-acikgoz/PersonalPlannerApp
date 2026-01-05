using System.Collections.ObjectModel;
using System.Windows.Input;
using PersonalPlannerApp.Models;

namespace PersonalPlannerApp.ViewModels;

public class MindViewModel : BindableObject
{
    private readonly LocalDbService _dbService;
    private ObservableCollection<MindItem> _mindItems;
    private bool _isDescending = true;
    private bool _isPopupVisible;
    private string _popupTitle;
    private string _popupContent;
    private string _selectedColor;
    private MindItem _editingItem;

    public ObservableCollection<MindItem> MindItems
    {
        get => _mindItems;
        set { _mindItems = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsListEmpty)); }
    }

    public bool IsListEmpty => MindItems == null || MindItems.Count == 0;
    public string SortText => _isDescending ? "Newest First" : "Oldest First";
    public bool IsPopupVisible { get => _isPopupVisible; set { _isPopupVisible = value; OnPropertyChanged(); } }
    public string PopupTitle { get => _popupTitle; set { _popupTitle = value; OnPropertyChanged(); } }
    public string PopupContent { get => _popupContent; set { _popupContent = value; OnPropertyChanged(); } }
    public string SelectedColor { get => _selectedColor; set { _selectedColor = value; OnPropertyChanged(); } }

    public List<string> ColorOptions { get; } = new List<string> { "#D1E9F6", "#F6EACB", "#F1D3CE", "#E2F1E7", "#E0E0E0" };

    public ICommand AddItemCommand { get; }
    public ICommand DeleteItemCommand { get; }
    public ICommand EditItemCommand { get; }
    public ICommand SaveItemCommand { get; }
    public ICommand ClosePopupCommand { get; }
    public ICommand ToggleSortCommand { get; }
    public ICommand SelectColorCommand { get; }

    public MindViewModel(LocalDbService dbService)
    {
        _dbService = dbService;
        MindItems = new ObservableCollection<MindItem>();

        AddItemCommand = new Command(() => {
            _editingItem = null;
            PopupTitle = string.Empty;
            PopupContent = string.Empty;
            SelectedColor = "#E0E0E0";
            IsPopupVisible = true;
        });

        SelectColorCommand = new Command<string>((color) => SelectedColor = color);

        SaveItemCommand = new Command(async () => {
            if (string.IsNullOrWhiteSpace(PopupTitle) && string.IsNullOrWhiteSpace(PopupContent)) return;
            if (_editingItem == null) {
                await _dbService.SaveMindItemAsync(new MindItem { 
                    Title = PopupTitle, Content = PopupContent, CreatedDate = DateTime.Now, ColorCode = SelectedColor 
                });
            } else {
                _editingItem.Title = PopupTitle; _editingItem.Content = PopupContent; _editingItem.ColorCode = SelectedColor;
                await _dbService.SaveMindItemAsync(_editingItem);
            }
            IsPopupVisible = false;
            await LoadItems();
        });

        // CANCEL MANTIĞI: Popup'ı kapat ve edit nesnesini temizle
        ClosePopupCommand = new Command(() => { 
            IsPopupVisible = false; 
            _editingItem = null; 
        });

        DeleteItemCommand = new Command<MindItem>(async (item) => {
            if (await Shell.Current.DisplayAlert("Delete", "Delete this note?", "Yes", "No")) {
                await _dbService.DeleteMindItemAsync(item);
                await LoadItems();
            }
        });

        EditItemCommand = new Command<MindItem>((item) => {
            _editingItem = item; PopupTitle = item.Title; PopupContent = item.Content;
            SelectedColor = item.ColorCode; IsPopupVisible = true;
        });

        ToggleSortCommand = new Command(async () => {
            _isDescending = !_isDescending; OnPropertyChanged(nameof(SortText)); await LoadItems();
        });

        _ = LoadItems();
    }

    private async Task LoadItems()
    {
        var items = await _dbService.GetMindItemsAsync();
        var sorted = _isDescending ? items.OrderByDescending(x => x.CreatedDate) : items.OrderBy(x => x.CreatedDate);
        MainThread.BeginInvokeOnMainThread(() => { MindItems = new ObservableCollection<MindItem>(sorted); });
    }
}