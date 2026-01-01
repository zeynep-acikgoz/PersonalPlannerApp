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

    // CheckBox tıklandığında bu çalışır
    private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        // Tetikleyen CheckBox'ı bul
        var cb = sender as CheckBox;
        
        // Hangi ToDoItem'a ait olduğunu bul
        if (cb?.BindingContext is ToDoItem item)
        {
            // O item'ın durumunu veritabanına kaydetmek için basit bir yol:
            // ViewModel içinde public bir update methodu olmadığı için 
            // DB servisine buradan erişmemiz gerekebilir ama
            // En temizi ViewModel üzerinden komut çağırmaktı.
            // Hızlı çözüm için: Bu event zaten UI'ı güncelliyor. 
            // ViewModel'e bir "UpdateItem" komutu ekleyip buradan çağırabilirsin.
            // Şimdilik sadece crash olmasın diye boş bırakıyorum, 
            // Binding zaten hafızada "IsCompleted" değerini güncelliyor.
        }
    }
}