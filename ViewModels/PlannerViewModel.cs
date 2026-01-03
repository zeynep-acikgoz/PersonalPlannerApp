using System.Collections.ObjectModel;
using System.Windows.Input;
using PersonalPlannerApp.Models;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Graphics;

namespace PersonalPlannerApp.ViewModels;

public class PlannerViewModel : BindableObject, IQueryAttributable
{
    private readonly LocalDbService _dbService;

    //--------------

    public ObservableCollection<CalendarModel> CalendarDays { get; set; } = new();

    private CalendarModel _selectedDay;
    public CalendarModel SelectedDay
    {
        get => _selectedDay;
        set
        {
            if (_selectedDay != value)
            {
                if (_selectedDay != null) _selectedDay.IsSelected = false;
                _selectedDay = value;
                if (_selectedDay != null) _selectedDay.IsSelected = true;

                OnPropertyChanged();
                OnPropertyChanged(nameof(TimelineTitle)); 
                
                if (_selectedDay != null)
                    Task.Run(() => LoadPlans(_selectedDay.Date));
            }
        }
    }

    public string TimelineTitle
    {
        get
        {
            if (SelectedDay == null) return "Timeline";
            
            if (SelectedDay.Date.Date == DateTime.Today)
                return "Today's Timeline";
            
            return $"{SelectedDay.Date:dd MMM} Timeline";
        }
    }

    //--------------

    private ObservableCollection<PlannerItem> _plannerItems;
    public ObservableCollection<PlannerItem> PlannerItems
    {
        get => _plannerItems;
        set { _plannerItems = value; OnPropertyChanged(); }
    }

    //--------------

    private bool _isPopupVisible;
    public bool IsPopupVisible
    {
        get => _isPopupVisible;
        set { _isPopupVisible = value; OnPropertyChanged(); }
    }

    private string _entryTitle;
    public string EntryTitle
    {
        get => _entryTitle;
        set { _entryTitle = value; OnPropertyChanged(); }
    }
    
    private DateTime _entryDate;
    public DateTime EntryDate
    {
        get => _entryDate;
        set { _entryDate = value; OnPropertyChanged(); }
    }

    private TimeSpan _entryStartTime;
    public TimeSpan EntryStartTime
    {
        get => _entryStartTime;
        set 
        { 
            _entryStartTime = value; 
            OnPropertyChanged(); 
            EntryEndTime = _entryStartTime.Add(TimeSpan.FromHours(1));
        }
    }

    private TimeSpan _entryEndTime;
    public TimeSpan EntryEndTime
    {
        get => _entryEndTime;
        set { _entryEndTime = value; OnPropertyChanged(); }
    }

    private PlannerItem _editingItem;

    //--------------

    public ICommand SelectDateCommand { get; }
    public ICommand DeletePlanCommand { get; }
    public ICommand TogglePlanCompleteCommand { get; }
    public ICommand OpenAddPopupCommand { get; }
    public ICommand ClosePopupCommand { get; }
    public ICommand SaveEntryCommand { get; }
    public ICommand EditPlanCommand { get; }
    public ICommand SendToToDoCommand { get; }

    public PlannerViewModel(LocalDbService dbService)
    {
        _dbService = dbService;
        PlannerItems = new ObservableCollection<PlannerItem>();

        GenerateCalendar();

        SelectDateCommand = new Command<CalendarModel>((day) => SelectedDay = day);
        
        DeletePlanCommand = new Command<PlannerItem>(async (item) => 
        {
            bool answer = await Shell.Current.DisplayAlert("Delete", "Are you sure?", "Yes", "No");
            if(answer)
            {
                await _dbService.DeletePlannerItemAsync(item);
                await LoadPlans(SelectedDay.Date);
            }
        });

        TogglePlanCompleteCommand = new Command<PlannerItem>(async (item) =>
        {
            await _dbService.SavePlannerItemAsync(item);
        });
        
        OpenAddPopupCommand = new Command(() =>
        {
            _editingItem = null;
            EntryTitle = string.Empty;
            EntryDate = SelectedDay != null ? SelectedDay.Date : DateTime.Today; 
            EntryStartTime = DateTime.Now.TimeOfDay;
            EntryEndTime = DateTime.Now.AddHours(1).TimeOfDay;
            IsPopupVisible = true;
        });

        EditPlanCommand = new Command<PlannerItem>((item) =>
        {
            _editingItem = item;
            EntryTitle = item.Title;
            EntryDate = item.Date; 
            EntryStartTime = item.StartTime;
            EntryEndTime = item.EndTime;
            IsPopupVisible = true;
        });

        ClosePopupCommand = new Command(() => IsPopupVisible = false);

        SaveEntryCommand = new Command(async () => await PerformSaveEntry());

        SendToToDoCommand = new Command<PlannerItem>(async (item) => await PerformSendToToDo(item));
    }

    private void GenerateCalendar()
    {
        var today = DateTime.Today;
        for (int i = 0; i < 30; i++) 
        {
            var date = today.AddDays(i);
            CalendarDays.Add(new CalendarModel
            {
                Date = date,
                DayName = date.ToString("ddd"),
                DayNumber = date.Day.ToString(),
                IsSelected = i == 0 
            });
        }
        SelectedDay = CalendarDays[0];
    }

    public async Task LoadPlans(DateTime date)
    {
        var items = await _dbService.GetPlannerItemsForDateAsync(date);
        var sorted = items.OrderBy(x => x.StartTime).ToList();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            PlannerItems = new ObservableCollection<PlannerItem>(sorted);
        });
    }

    private async Task PerformSaveEntry()
    {
        if (string.IsNullOrWhiteSpace(EntryTitle))
        {
            await Shell.Current.DisplayAlert("Warning", "Title required.", "OK");
            return;
        }

        if (EntryEndTime < EntryStartTime)
        {
            await Shell.Current.DisplayAlert("Error", "End time must be after start time.", "OK");
            return;
        }

        if (_editingItem == null)
        {
            var newItem = new PlannerItem
            {
                Title = EntryTitle,
                Date = EntryDate, 
                StartTime = EntryStartTime,
                EndTime = EntryEndTime,
                IsCompleted = false
            };
            await _dbService.SavePlannerItemAsync(newItem);
        }
        else
        {
            _editingItem.Title = EntryTitle;
            _editingItem.Date = EntryDate;
            _editingItem.StartTime = EntryStartTime;
            _editingItem.EndTime = EntryEndTime;
            await _dbService.SavePlannerItemAsync(_editingItem);
        }

        IsPopupVisible = false;
        await LoadPlans(SelectedDay.Date);
    }

    //--------------

    private async Task PerformSendToToDo(PlannerItem item)
    {
        if (item == null) return;

        var navParam = new Dictionary<string, object>
        {
            { "PlanTitle", item.Title }
        };
        
        await Shell.Current.GoToAsync("//ToDoPage", navParam);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("TaskTitle"))
        {
            var title = query["TaskTitle"] as string;
            
            _editingItem = null; 
            EntryTitle = title;  
            EntryDate = SelectedDay != null ? SelectedDay.Date : DateTime.Today; 
            EntryStartTime = DateTime.Now.TimeOfDay;
            EntryEndTime = DateTime.Now.AddHours(1).TimeOfDay;
            
            IsPopupVisible = true; 
        }
    }
}

//--------------

public class CalendarModel : BindableObject
{
    public DateTime Date { get; set; }
    public string DayName { get; set; }
    public string DayNumber { get; set; }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set 
        { 
            _isSelected = value; 
            OnPropertyChanged();
            OnPropertyChanged(nameof(BackgroundColor));
            OnPropertyChanged(nameof(TextColor));
        }
    }

    public Color BackgroundColor => IsSelected ? Colors.CornflowerBlue : Colors.Transparent;
    public Color TextColor => IsSelected ? Colors.White : Colors.Gray;
}