using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel; // Tema yönetimi için gerekli

namespace PersonalPlannerApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
        
        UpdateDate();
        UpdateGreeting();
        CheckBriefVisibility();
        
        // Uygulama ilk açıldığında, Switch'in konumunu mevcut temaya göre ayarla
        // Eğer şu an Dark mod ise, Switch açık (True) olsun.
        if (Application.Current.RequestedTheme == AppTheme.Dark)
        {
            ThemeSwitch.IsToggled = true;
        }
    }

    // Kullanıcı Switch butonuna bastığında bu çalışır
    private void OnThemeSwitchToggled(object sender, ToggledEventArgs e)
    {
        // Switch açık mı (True) kapalı mı (False)?
        bool isDarkMode = e.Value;

        if (isDarkMode)
        {
            // Uygulamayı KARANLIK moda zorla
            Application.Current.UserAppTheme = AppTheme.Dark;
        }
        else
        {
            // Uygulamayı AYDINLIK moda zorla
            Application.Current.UserAppTheme = AppTheme.Light;
        }
    }

    private void OnTimeChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Time")
        {
            CheckBriefVisibility();
        }
    }

    private void CheckBriefVisibility()
    {
        TimeSpan currentTime = DateTime.Now.TimeOfDay;
        TimeSpan eveningThreshold = PickerEvening.Time;

        if (currentTime >= eveningThreshold)
            FrameEvening.IsVisible = true;
        else
            FrameEvening.IsVisible = false;
    }

    private void UpdateGreeting()
    {
        var currentHour = DateTime.Now.Hour;
        string greetingText = "";

        if (currentHour >= 5 && currentHour < 12) greetingText = "Good morning";
        else if (currentHour >= 12 && currentHour < 17) greetingText = "Good afternoon";
        else if (currentHour >= 17 && currentHour < 21) greetingText = "Good evening";
        else greetingText = "Good night";

        LblGreeting.Text = greetingText;
    }

    private void UpdateDate()
    {
        LblDate.Text = DateTime.Now.ToString("d MMMM dddd", CultureInfo.GetCultureInfo("en-US"));
    }
}