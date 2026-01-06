using System.Collections.ObjectModel;
using System.Windows.Input;
using PersonalPlannerApp.Models;

namespace PersonalPlannerApp.ViewModels;

public class AssistantViewModel : BindableObject
{
    private string _userInput;
    public string UserInput { get => _userInput; set { _userInput = value; OnPropertyChanged(); } }

    public ObservableCollection<ChatMessage> Messages { get; } = new();
    public List<string> SuggestedQuestions { get; } = new()
    {
        "Do I have any exams this week?",
        "Show my deadlines for next week.",
        "Find the song lyrics I noted down."
    };

    public ICommand SendCommand { get; }
    public ICommand SuggestionCommand { get; }

    public AssistantViewModel()
    {
        Messages.Add(new ChatMessage { Text = "Hello! How can I help you today?", IsUser = false });

        SuggestionCommand = new Command<string>((q) => UserInput = q);

        SendCommand = new Command(async () =>
        {
            if (string.IsNullOrWhiteSpace(UserInput)) return;

            var userText = UserInput;
            Messages.Add(new ChatMessage { Text = userText, IsUser = true });
            UserInput = string.Empty;

            await Task.Delay(1000);
            Messages.Add(new ChatMessage 
            { 
                Text = $"I'm analyzing your data about '{userText}'. I'll find the best result for you.", 
                IsUser = false 
            });
        });
    }
}