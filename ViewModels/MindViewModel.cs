using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PersonalPlannerApp.ViewModels;

public class MindViewModel : BindableObject
{
    private ObservableCollection<string> _mindItems;
    public ObservableCollection<string> MindItems
    {
        get => _mindItems;
        set { _mindItems = value; OnPropertyChanged(); }
    }

    public ICommand AddItemCommand { get; }

    public MindViewModel()
    {
        MindItems = new ObservableCollection<string>();

        
        AddItemCommand = new Command(() =>
        {
            MindItems.Add($"New Idea {DateTime.Now.Second}");
        });
    }
}