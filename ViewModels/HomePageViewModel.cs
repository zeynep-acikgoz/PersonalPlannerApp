using System.Windows.Input;
using System.Globalization;

namespace PersonalPlannerApp.ViewModels;

public class HomePageViewModel : BindableObject
{
    
    public string CurrentDateDisplay => DateTime.Now.ToString("dddd, d MMMM", CultureInfo.GetCultureInfo("en-US"));

    public ICommand AddTodoCommand { get; }
    public ICommand CreatePlanCommand { get; }
    public ICommand AskAssistantCommand { get; }
    public ICommand AddNoteCommand { get; }

    public HomePageViewModel()
    {
        
        AddTodoCommand = new Command(async () => await Shell.Current.GoToAsync("//ToDoPage"));
        CreatePlanCommand = new Command(async () => await Shell.Current.GoToAsync("//PlanPage"));
        AskAssistantCommand = new Command(async () => await Shell.Current.GoToAsync("//AssistantPage"));
        AddNoteCommand = new Command(async () => await Shell.Current.GoToAsync("//MindPage"));
    }
}