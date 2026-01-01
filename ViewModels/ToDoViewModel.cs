using System.Collections.ObjectModel;
using System.Windows.Input;
using PersonalPlannerApp.Models;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Graphics;

namespace PersonalPlannerApp.ViewModels;

public class ToDoViewModel : BindableObject
{
    private readonly LocalDbService _dbService;

    // --- DEĞİŞİKLİK 1: Listenin referansını değiştireceğimiz için Full Property yapıyoruz ---
    private ObservableCollection<ToDoGroup> _groupedTasks;
    public ObservableCollection<ToDoGroup> GroupedTasks
    {
        get => _groupedTasks;
        set
        {
            _groupedTasks = value;
            OnPropertyChanged(); // Ekran, listenin tamamen değiştiğini anlar
        }
    }

    // --- GİRİŞ ALANLARI ---
    private string _newTaskText;
    public string NewTaskText
    {
        get => _newTaskText;
        set { _newTaskText = value; OnPropertyChanged(); }
    }

    // KATEGORİLER
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

    // ÖNCELİKLER
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

    // TARİH
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

    // --- KOMUTLAR ---
    public ICommand AddTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand CycleCategoryCommand { get; }
    public ICommand CyclePriorityCommand { get; }
    public ICommand ToggleGroupCommand { get; }
    // Not: ToggleCompleteCommand'i CheckBox'ın kendi özelliğiyle yöneteceğiz veya gerekirse ekleriz.

    public ToDoViewModel(LocalDbService dbService)
    {
        _dbService = dbService;
        
        // Başlangıçta boş bir liste ata
        GroupedTasks = new ObservableCollection<ToDoGroup>();

        AddTaskCommand = new Command(async () => await PerformAddTask());
        DeleteTaskCommand = new Command<ToDoItem>(async (item) => await PerformDeleteTask(item));
        RefreshCommand = new Command(async () => await LoadTasks());

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

        // Uygulama açılır açılmaz verileri çek
        Task.Run(LoadTasks);
    }

    // --- DEĞİŞİKLİK 2: KESİN ÇÖZÜM BURADA ---
    // Listeyi silmek (Clear) yerine yeni bir liste oluşturup atıyoruz (Swap).
    private async Task LoadTasks()
    {
        try
        {
            var allTasks = await _dbService.GetTasksAsync();

            // Verileri arka planda hazırla (UI thread'i yorma)
            var sorted = allTasks.OrderBy(t => t.IsCompleted).ThenBy(t => t.DueDate).ToList();
            
            var high = sorted.Where(t => t.PriorityLevel == 2).ToList();
            var medium = sorted.Where(t => t.PriorityLevel == 1).ToList();
            var low = sorted.Where(t => t.PriorityLevel == 0).ToList();

            // Geçici bir koleksiyon oluştur
            var newCollection = new ObservableCollection<ToDoGroup>();

            if (high.Any()) newCollection.Add(new ToDoGroup("High Priority 🔥", high));
            if (medium.Any()) newCollection.Add(new ToDoGroup("Medium Priority ⚡", medium));
            if (low.Any()) newCollection.Add(new ToDoGroup("Low Priority ☕", low));

            // UI'ı güncelle
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Clear() ve Add() YAPMIYORUZ. Direkt yeni listeyi atıyoruz.
                // Bu sayede "Collection modified" hatası alman imkansızlaşır.
                GroupedTasks = newCollection; 
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadTasks Hatası: {ex.Message}");
        }
    }

    private async Task PerformAddTask()
    {
        if (string.IsNullOrWhiteSpace(NewTaskText)) return;

        try
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

            NewTaskText = string.Empty;
            HasDueDate = false;

            // Klavye kapanma süresi için bekleme (Crash önleyici)
            await Task.Delay(250); 

            await LoadTasks(); 
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Hata", ex.Message, "Tamam");
        }
    }

    private async Task PerformDeleteTask(ToDoItem item)
    {
        if (item == null) return;
        await _dbService.DeleteTaskAsync(item);
        await LoadTasks(); // Listeyi yenile
    }
}