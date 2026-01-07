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

    
        builder.Services.AddSingleton<ToDoViewModel>();
        builder.Services.AddSingleton<PlannerViewModel>();
        builder.Services.AddSingleton<MindViewModel>(); 
        
        builder.Services.AddTransient<AssistantPage>();
        builder.Services.AddTransient<AssistantViewModel>();
        builder.Services.AddSingleton<HomePage>(); 
        builder.Services.AddSingleton<ToDoPage>();
        builder.Services.AddSingleton<PlanPage>();
        builder.Services.AddSingleton<MindPage>(); 
        builder.Services.AddSingleton<AssistantPage>();
        

        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<HomePageViewModel>();

        return builder.Build();
    }
}