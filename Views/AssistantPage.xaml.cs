using PersonalPlannerApp.ViewModels;

namespace PersonalPlannerApp.Views;

public partial class AssistantPage : ContentPage
{
    
    public AssistantPage(AssistantViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
    }
}