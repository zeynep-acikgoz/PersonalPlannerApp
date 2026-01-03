using System.Collections.ObjectModel;
using System.Windows.Input;
using PersonalPlannerApp.Models;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Graphics;

namespace PersonalPlannerApp.ViewModels;

public class ToDoViewModel : BindableObject, IQueryAttributable
{
    private readonly LocalDbService _dbService;
    
    //--------------
    
    private ObservableCollection<ToDoGroup> _groupedTasks;
    public ObservableCollection<ToDoGroup> GroupedTasks
    {
        get => _groupedTasks;
        set { _groupedTasks = value; OnPropertyChanged(); }
    }

    //--------------
    
    private bool _isPopupVisible;
    public bool IsPopupVisible
    {
        get => _isPopupVisible;
        set { _isPopupVisible = value; OnPropertyChanged(); }
    }

    private string _popupTitleText;
    public string PopupTitleText
    {
        get => _popupTitleText;
        set { _popupTitleText = value; OnPropertyChanged(); }
    }

    private string _popupEntryText;
    public string PopupEntryText
    {
        get => _popupEntryText;
        set { _popupEntryText = value; OnPropertyChanged(); }
    }

    private ToDoItem _editingItem;

    //--------------
    
    private readonly string[] _categories = { "School", "Work", "Personal" };
    private int _categoryIndex = 0;
    public string SelectedCategory => _categories[_categoryIndex];
    public Color CategoryButtonColor => SelectedCategory switch
    {
        "School" => Colors.CornflowerBlue,
        "Work" => Colors.MediumPurple,
        "Personal" => Colors.HotPink,
        _ => Colors.Gray
    };

    private readonly string[] _priorities = { "Low", "Medium", "High" };
    private int _priorityIndex = 2; 
    public string SelectedPriority => _priorities[_priorityIndex];
    public Color PriorityButtonColor => SelectedPriority switch
    {
        "High" => Colors.OrangeRed,
        "Medium" => Colors.Orange,
        "Low" => Colors.SeaGreen,
        _ => Colors.Gray
    };

    private bool _hasDueDate;
    public bool HasDueDate
    {
        get => _hasDueDate;
        set { _hasDueDate = value; OnPropertyChanged(); }
    }

    private DateTime _selectedDate = DateTime.Now;
    public DateTime SelectedDate
    {
        get => _selectedDate;
        set { _selectedDate = value; OnPropertyChanged(); }
    }

    //--------------
    
    public ICommand SaveTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand ToggleCompleteCommand { get; }
    public ICommand OpenAddPopupCommand { get; }
    public ICommand ClosePopupCommand { get; }
    public ICommand EditTaskCommand { get; }
    public ICommand CycleCategoryCommand { get; }
    public ICommand CyclePriorityCommand { get; }
    public ICommand ToggleGroupCommand { get; }
    public ICommand PlanTaskCommand { get; }

    public ToDoViewModel(LocalDbService dbService)
    {
        _dbService = dbService;
        GroupedTasks = new ObservableCollection<ToDoGroup>();

        SaveTaskCommand = new Command(async () => await PerformSaveTask());

        OpenAddPopupCommand = new Command(() =>
        {
            _editingItem = null;
            PopupTitleText = "New Task";
            PopupEntryText = string.Empty;
            HasDueDate = false;
            _categoryIndex = 0; UpdateCategoryUI();
            _priorityIndex = 2; UpdatePriorityUI();
            IsPopupVisible = true;
        });

        ClosePopupCommand = new Command(() => IsPopupVisible = false);

        EditTaskCommand = new Command<ToDoItem>((item) =>
        {
            if (item == null) return;
            _editingItem = item;
            PopupTitleText = "Edit Task";
            PopupEntryText = item.Title;
            HasDueDate = item.DueDate.HasValue;
            if (item.DueDate.HasValue) SelectedDate = item.DueDate.Value;
            
            for (int i = 0; i < _categories.Length; i++) {
                if (_categories[i] == item.Category) { _categoryIndex = i; break; }
            }
            UpdateCategoryUI();

            for (int i = 0; i < _priorities.Length; i++) {
                if (i == item.PriorityLevel) { _priorityIndex = i; break; }
            }
            UpdatePriorityUI();

            IsPopupVisible = true;
        });

        DeleteTaskCommand = new Command<ToDoItem>(async (item) => await PerformDeleteTask(item));
        ToggleCompleteCommand = new Command<ToDoItem>(async (item) => await _dbService.SaveTaskAsync(item));
        
        PlanTaskCommand = new Command<ToDoItem>(async (item) => 
        {
            if (item == null) return;
            var navParam = new Dictionary<string, object> { { "TaskTitle", item.Title } };
            await Shell.Current.GoToAsync("//PlanPage", navParam);
        });

        CycleCategoryCommand = new Command(() => {
            _categoryIndex = (_categoryIndex + 1) % _categories.Length;
            UpdateCategoryUI();
        });

        CyclePriorityCommand = new Command(() => {
            _priorityIndex = (_priorityIndex + 1) % _priorities.Length;
            UpdatePriorityUI();
        });

        ToggleGroupCommand = new Command<ToDoGroup>((group) => { if (group != null) group.IsExpanded = !group.IsExpanded; });
        
        Task.Run(LoadTasks);
    }

    private void UpdateCategoryUI() { OnPropertyChanged(nameof(SelectedCategory)); OnPropertyChanged(nameof(CategoryButtonColor)); }
    private void UpdatePriorityUI() { OnPropertyChanged(nameof(SelectedPriority)); OnPropertyChanged(nameof(PriorityButtonColor)); }

    //--------------
    
    private async Task LoadTasks()
    {
        var allTasks = await _dbService.GetTasksAsync();
        var sorted = allTasks.OrderBy(t => t.IsCompleted).ThenBy(t => t.DueDate).ToList();
        
        var high = sorted.Where(t => t.PriorityLevel == 2).ToList();
        var medium = sorted.Where(t => t.PriorityLevel == 1).ToList();
        var low = sorted.Where(t => t.PriorityLevel == 0).ToList();

        var newCollection = new ObservableCollection<ToDoGroup>();
        if (high.Any()) newCollection.Add(new ToDoGroup("High Priority 🔥", high));
        if (medium.Any()) newCollection.Add(new ToDoGroup("Medium Priority ⚡", medium));
        if (low.Any()) newCollection.Add(new ToDoGroup("Low Priority ☕", low));

        MainThread.BeginInvokeOnMainThread(() => GroupedTasks = newCollection);
    }

    private async Task PerformSaveTask()
    {
        if (string.IsNullOrWhiteSpace(PopupEntryText))
        {
            await Shell.Current.DisplayAlert("Warning", "Please enter a task title.", "OK");
            return;
        }

        if (_editingItem == null)
        {
            var newTask = new ToDoItem
            {
                Title = PopupEntryText,
                IsCompleted = false,
                DueDate = HasDueDate ? SelectedDate : (DateTime?)null,
                Category = SelectedCategory,
                PriorityLevel = _priorityIndex
            };
            await _dbService.SaveTaskAsync(newTask);
        }
        else
        {
            _editingItem.Title = PopupEntryText;
            _editingItem.DueDate = HasDueDate ? SelectedDate : (DateTime?)null;
            _editingItem.Category = SelectedCategory;
            _editingItem.PriorityLevel = _priorityIndex;
            await _dbService.SaveTaskAsync(_editingItem);
        }

        IsPopupVisible = false;
        PopupEntryText = string.Empty;
        await Task.Delay(200);
        await LoadTasks();
    }

    private async Task PerformDeleteTask(ToDoItem item)
    {
        if (item == null) return;
        await _dbService.DeleteTaskAsync(item);
        await LoadTasks();
    }

    //--------------

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("PlanTitle"))
        {
            var title = query["PlanTitle"] as string;
            
            _editingItem = null;
            PopupTitleText = "New Task from Plan";
            PopupEntryText = title;
            HasDueDate = false;
            
            IsPopupVisible = true;
        }
    }
}