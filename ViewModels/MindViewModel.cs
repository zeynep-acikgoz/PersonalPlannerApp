using System.Collections.ObjectModel;
using System.Windows.Input;
using PersonalPlannerApp.Models;

namespace PersonalPlannerApp.ViewModels;

public class MindViewModel : BindableObject
{
    private readonly LocalDbService _dbService;

    
    private ObservableCollection<MindItem> _mindItems;
    public ObservableCollection<MindItem> MindItems
    {
        get => _mindItems;
        set { _mindItems = value; OnPropertyChanged(); }
    }

    private bool _isDescending = true;
    public string SortText => _isDescending ? "Newest First" : "Oldest First";

    
    private bool _isPopupVisible;
    public bool IsPopupVisible
    {
        get => _isPopupVisible;
        set { _isPopupVisible = value; OnPropertyChanged(); }
    }

    private string _popupTitle;
    public string PopupTitle
    {
        get => _popupTitle;
        set { _popupTitle = value; OnPropertyChanged(); }
    }

    private string _popupContent;
    public string PopupContent
    {
        get => _popupContent;
        set { _popupContent = value; OnPropertyChanged(); }
    }

    private string _selectedColor;
    public string SelectedColor
    {
        get => _selectedColor;
        set { _selectedColor = value; OnPropertyChanged(); }
    }

   
    public List<string> ColorOptions { get; } = new List<string>
    {
        "#D1E9F6", // Soft Mavi
        "#F6EACB", // Soft Sarı
        "#F1D3CE", // Soft Kırmızı
        "#E2F1E7", // Soft Yeşil
        "#E0E0E0"  // Soft Gri 
    };

    private MindItem _editingItem;

    //--------------------
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

        AddItemCommand = new Command(() =>
        {
            _editingItem = null;
            PopupTitle = "";
            PopupContent = "";
            SelectedColor = "#E0E0E0"; // Başlangıçta gri
            IsPopupVisible = true;
        });

        SelectColorCommand = new Command<string>((color) => SelectedColor = color);

        SaveItemCommand = new Command(async () =>
        {
            if (string.IsNullOrWhiteSpace(PopupTitle) && string.IsNullOrWhiteSpace(PopupContent))
            {
                await Shell.Current.DisplayAlert("Warning", "Cannot save an empty note.", "OK");
                return;
            }

            try
            {
                if (_editingItem == null)
                {
                    var newItem = new MindItem
                    {
                        Title = PopupTitle,
                        Content = PopupContent,
                        CreatedDate = DateTime.Now,
                        ColorCode = SelectedColor
                    };
                    await _dbService.SaveMindItemAsync(newItem);
                }
                else
                {
                    _editingItem.Title = PopupTitle;
                    _editingItem.Content = PopupContent;
                    _editingItem.ColorCode = SelectedColor;
                    await _dbService.SaveMindItemAsync(_editingItem);
                }

                IsPopupVisible = false;
                await LoadItems();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Save failed: " + ex.Message, "Close");
            }
        });

        DeleteItemCommand = new Command<MindItem>(async (item) =>
        {
            bool answer = await Shell.Current.DisplayAlert("Delete", "Are you sure you want to delete this?", "Yes", "No");
            if (answer)
            {
                await _dbService.DeleteMindItemAsync(item);
                await LoadItems();
            }
        });

        EditItemCommand = new Command<MindItem>((item) =>
        {
            _editingItem = item;
            PopupTitle = item.Title;
            PopupContent = item.Content;
            SelectedColor = item.ColorCode;
            IsPopupVisible = true;
        });

        ClosePopupCommand = new Command(() => IsPopupVisible = false);

        ToggleSortCommand = new Command(async () =>
        {
            _isDescending = !_isDescending;
            OnPropertyChanged(nameof(SortText));
            await LoadItems();
        });

        
        _ = LoadItems();
    }

   
    private async Task LoadItems()
    {
        var items = await _dbService.GetMindItemsAsync();
        
        IEnumerable<MindItem> sorted;
        if (_isDescending)
            sorted = items.OrderByDescending(x => x.CreatedDate);
        else
            sorted = items.OrderBy(x => x.CreatedDate);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            MindItems = new ObservableCollection<MindItem>(sorted);
        });
    }
}