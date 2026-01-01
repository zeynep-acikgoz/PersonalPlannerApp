using Microsoft.Extensions.Logging;

namespace PersonalPlannerApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Veritabanı servisini ve Sayfaları sisteme tanıtıyoruz
        builder.Services.AddSingleton<LocalDbService>();

        builder.Services.AddTransient<PersonalPlannerApp.ViewModels.ToDoViewModel>();
        builder.Services.AddTransient<PersonalPlannerApp.Views.ToDoPage>();
        return builder.Build();
    }
}