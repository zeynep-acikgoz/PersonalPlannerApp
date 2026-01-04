using PersonalPlannerApp.ViewModels;

namespace PersonalPlannerApp.Views;

public partial class MindPage : ContentPage
{
    public MindPage(MindViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}