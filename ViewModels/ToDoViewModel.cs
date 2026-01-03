using System.Collections.ObjectModel;
using System.Windows.Input;
using PersonalPlannerApp.Models;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Graphics;

namespace PersonalPlannerApp.ViewModels;

public class ToDoViewModel : BindableObject
{
    private readonly LocalDbService _dbService;
    
    private ObservableCollection<ToDoGroup> _groupedTasks;
    public ObservableCollection<ToDoGroup> GroupedTasks
    {
        get => _groupedTasks;
        set
        {
            _groupedTasks = value;
            OnPropertyChanged();
        }
    }

    private string _newTaskText;
    public string NewTaskText
    {
        get => _newTaskText;
        set { _newTaskText = value; OnPropertyChanged(); }
    }

    // -------------------
    
    private ToDoItem _editItem; 
    
    public bool IsEditing => _editItem != null;
    
    public string StateButtonText => _editItem == null ? "+" : "OK";

    // ----------------
    
    private readonly string[] _categories = { "School", "Work", "Personal" };
    private int _categoryIndex = 0;
    public string SelectedCategory => _categories[_categoryIndex];

    public Color CategoryButtonColor
    {
        get
        {
            return SelectedCategory switch
            {
                "School" => Colors.CornflowerBlue,
                "Work" => Colors.MediumPurple,
                "Personal" => Colors.HotPink,
                _ => Colors.Gray
            };
        }
    }

    // ------------------
    
    private readonly string[] _priorities = { "Low", "Medium", "High" };
    private int _priorityIndex = 2; 
    public string SelectedPriority => _priorities[_priorityIndex];

    public Color PriorityButtonColor
    {
        get
        {
            return SelectedPriority switch
            {
                "High" => Colors.OrangeRed,
                "Medium" => Colors.Orange,
                "Low" => Colors.SeaGreen,
                _ => Colors.Gray
            };
        }
    }

    // -------------
    
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

    // ---------------
    
    public ICommand AddTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand ToggleCompleteCommand { get; }
    public ICommand EditTaskCommand { get; }
    public ICommand CycleCategoryCommand { get; }
    public ICommand CyclePriorityCommand { get; }
    public ICommand ToggleGroupCommand { get; }
    
    // --- YENİ EKLENEN KOMUT: PLANLAMA ---
    public ICommand PlanTaskCommand { get; }
    
    // ----------
    
    public ToDoViewModel(LocalDbService dbService)
    {
        _dbService = dbService;
        
        GroupedTasks = new ObservableCollection<ToDoGroup>();

        AddTaskCommand = new Command(async () => await PerformAddTask());
        DeleteTaskCommand = new Command<ToDoItem>(async (item) => await PerformDeleteTask(item));
        RefreshCommand = new Command(async () => await LoadTasks());
        ToggleCompleteCommand = new Command<ToDoItem>(async (item) => await PerformToggleComplete(item));
        EditTaskCommand = new Command<ToDoItem>(PerformEditTask); 

        // --- YENİ PLANLAMA KOMUTU TANIMI ---
        PlanTaskCommand = new Command<ToDoItem>(async (item) => await PerformPlanTask(item));

        CycleCategoryCommand = new Command(() =>
        {
            _categoryIndex = (_categoryIndex + 1) % _categories.Length;
            OnPropertyChanged(nameof(SelectedCategory));
            OnPropertyChanged(nameof(CategoryButtonColor));
        });

        CyclePriorityCommand = new Command(() =>
        {
            _priorityIndex = (_priorityIndex + 1) % _priorities.Length;
            OnPropertyChanged(nameof(SelectedPriority));
            OnPropertyChanged(nameof(PriorityButtonColor));
        });

        ToggleGroupCommand = new Command<ToDoGroup>((group) =>
        {
            if (group != null)
                group.IsExpanded = !group.IsExpanded;
        });
        
        Task.Run(LoadTasks);
    }

    // ---------------------------
    private async Task LoadTasks()
    {
        try
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

            MainThread.BeginInvokeOnMainThread(() =>
            {
                GroupedTasks = newCollection; 
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadTasks Hatası: {ex.Message}");
        }
    }

    // ----------------------
    private async Task PerformDeleteTask(ToDoItem item)
    {
        if (item == null) return;
        await _dbService.DeleteTaskAsync(item);
        await LoadTasks();
    }
    
    // ------------------
    
    private async Task PerformToggleComplete(ToDoItem item)
    {
        await _dbService.SaveTaskAsync(item);
    }
    
    // ----------------------
    private void PerformEditTask(ToDoItem item)
    {
        if (item == null) return;

        _editItem = item; 
        
        NewTaskText = item.Title;
        HasDueDate = item.DueDate.HasValue;
        if (item.DueDate.HasValue) SelectedDate = item.DueDate.Value;
        
        for (int i = 0; i < _categories.Length; i++)
        {
            if (_categories[i] == item.Category)
            {
                _categoryIndex = i;
                OnPropertyChanged(nameof(SelectedCategory));
                OnPropertyChanged(nameof(CategoryButtonColor));
                break;
            }
        }
        
        for (int i = 0; i < _priorities.Length; i++)
        {
            if (i == item.PriorityLevel) 
            {
                _priorityIndex = i;
                OnPropertyChanged(nameof(SelectedPriority));
                OnPropertyChanged(nameof(PriorityButtonColor));
                break;
            }
        }
        
        OnPropertyChanged(nameof(StateButtonText));
        OnPropertyChanged(nameof(IsEditing)); 
    }

    // ----------------------------------------------------
    
    private async Task PerformAddTask()
    {
        if (string.IsNullOrWhiteSpace(NewTaskText))
        {
            await Shell.Current.DisplayAlert("Uyarı", "Lütfen bir görev adı giriniz.", "Tamam");
            return; 
        }

        try
        {
            if (_editItem == null)
            {
                var newTask = new ToDoItem
                {
                    Title = NewTaskText,
                    IsCompleted = false,
                    DueDate = HasDueDate ? SelectedDate : (DateTime?)null, 
                    Category = SelectedCategory,
                    PriorityLevel = _priorityIndex
                };
                await _dbService.SaveTaskAsync(newTask);
            }
            else
            {
                _editItem.Title = NewTaskText;
                _editItem.DueDate = HasDueDate ? SelectedDate : (DateTime?)null;
                _editItem.Category = SelectedCategory;
                _editItem.PriorityLevel = _priorityIndex;
                
                await _dbService.SaveTaskAsync(_editItem);
                
                _editItem = null; 
            }

            NewTaskText = string.Empty;
            HasDueDate = false;
            
            OnPropertyChanged(nameof(StateButtonText)); 
            OnPropertyChanged(nameof(IsEditing));   

            await Task.Delay(250); 
            await LoadTasks(); 
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Hata", ex.Message, "Tamam");
        }
    }

    
    private async Task PerformPlanTask(ToDoItem item)
    {
        if (item == null) return;

        
        var navigationParameter = new Dictionary<string, object>
        {
            { "TaskTitle", item.Title }
        };

        await Shell.Current.GoToAsync("//PlanPage", navigationParameter);
    }
}