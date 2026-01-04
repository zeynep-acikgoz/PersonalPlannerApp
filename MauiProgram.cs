using Microsoft.Extensions.Logging;
using PersonalPlannerApp.ViewModels;
using PersonalPlannerApp.Views;

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
        
        // 1. Veritabanı Servisi
        builder.Services.AddSingleton<LocalDbService>();

        // 2. ViewModels (Tek Seferlik Tanımlar)
        builder.Services.AddSingleton<ToDoViewModel>();
        builder.Services.AddSingleton<PlannerViewModel>();
        builder.Services.AddSingleton<MindViewModel>(); // Mind Eklendi

        // 3. Pages (Sayfalar)
        // Eğer HomePage ve AssistantPage varsa onları da buraya eklemelisin.
        builder.Services.AddSingleton<HomePage>(); 
        builder.Services.AddSingleton<ToDoPage>();
        builder.Services.AddSingleton<PlanPage>();
        builder.Services.AddSingleton<MindPage>(); // Mind Eklendi
        builder.Services.AddSingleton<AssistantPage>();

        return builder.Build();
    }
}