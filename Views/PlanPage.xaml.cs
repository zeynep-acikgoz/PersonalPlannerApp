using PersonalPlannerApp.ViewModels;

namespace PersonalPlannerApp.Views;

public partial class PlanPage : ContentPage
{
    public PlanPage(PlannerViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}