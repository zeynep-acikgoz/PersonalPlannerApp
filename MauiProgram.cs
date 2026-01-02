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
        
        builder.Services.AddSingleton<LocalDbService>();

      
        builder.Services.AddTransient<ToDoViewModel>();
        builder.Services.AddTransient<ToDoPage>();
        
        builder.Services.AddTransient<PlannerViewModel>();
        builder.Services.AddTransient<PlanPage>();

        return builder.Build();
    }
}