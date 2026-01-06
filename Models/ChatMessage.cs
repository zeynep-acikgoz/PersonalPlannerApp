namespace PersonalPlannerApp.Models;

public class ChatMessage
{
    public string Text { get; set; }
    public bool IsUser { get; set; }
    public DateTime MessageTime { get; set; } = DateTime.Now;
}