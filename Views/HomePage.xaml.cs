using PersonalPlannerApp.ViewModels;

namespace PersonalPlannerApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomePageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnThemeSwitchToggled(object sender, ToggledEventArgs e)
    {
        Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
    }
}