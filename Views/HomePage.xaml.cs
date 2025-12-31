using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PersonalPlannerApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
        
        UpdateDate();
        UpdateGreeting();
        CheckBriefVisibility(); 
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
        {
            FrameEvening.IsVisible = true; 
        }
        else
        {
            FrameEvening.IsVisible = false; 
        }
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