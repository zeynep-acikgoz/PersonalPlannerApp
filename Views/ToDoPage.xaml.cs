using PersonalPlannerApp.Models;
using PersonalPlannerApp.ViewModels;

namespace PersonalPlannerApp.Views;

public partial class ToDoPage : ContentPage
{
    private readonly ToDoViewModel _vm;

    public ToDoPage(ToDoViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var cb = sender as CheckBox;
        if (cb?.BindingContext is ToDoItem item)
        {
            
            if (_vm.ToggleCompleteCommand.CanExecute(item))
            {
                _vm.ToggleCompleteCommand.Execute(item);
            }
        }
    }
}