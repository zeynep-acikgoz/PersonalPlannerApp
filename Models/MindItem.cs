using SQLite;

namespace PersonalPlannerApp.Models;

[Table("mind_items")]
public class MindItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Title { get; set; }    
    public string Content { get; set; }   
    public DateTime CreatedDate { get; set; } 
}