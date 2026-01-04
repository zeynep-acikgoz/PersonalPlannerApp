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

    private MindItem _editingItem; 
    
    public ICommand AddItemCommand { get; }
    public ICommand DeleteItemCommand { get; }
    public ICommand EditItemCommand { get; }
    public ICommand SaveItemCommand { get; }
    public ICommand ClosePopupCommand { get; }

    public MindViewModel(LocalDbService dbService)
    {
        _dbService = dbService;
        MindItems = new ObservableCollection<MindItem>();

        AddItemCommand = new Command(() =>
        {
            _editingItem = null;
            PopupTitle = "";
            PopupContent = "";
            IsPopupVisible = true;
        });

        SaveItemCommand = new Command(async () =>
        {
            if (string.IsNullOrWhiteSpace(PopupTitle) && string.IsNullOrWhiteSpace(PopupContent))
            {
                await Shell.Current.DisplayAlert("Warning", "Cannot save an empty note.", "Okey");
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
                        CreatedDate = DateTime.Now
                    };
                    await _dbService.SaveMindItemAsync(newItem);
                }
                else
                {
                    _editingItem.Title = PopupTitle;
                    _editingItem.Content = PopupContent;
                    await _dbService.SaveMindItemAsync(_editingItem);
                }

                IsPopupVisible = false;
                await LoadItems();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("HATA", "Kayıt hatası: " + ex.Message, "Kapat");
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
            IsPopupVisible = true;
        });

        ClosePopupCommand = new Command(() => IsPopupVisible = false);

        
        Task.Run(LoadItems);
    }

    private async Task LoadItems()
    {
        var items = await _dbService.GetMindItemsAsync();
        var sorted = items.OrderByDescending(x => x.CreatedDate).ToList();
        
        MainThread.BeginInvokeOnMainThread(() =>
        {
            MindItems = new ObservableCollection<MindItem>(sorted);
        });
    }
}