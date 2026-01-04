using SQLite;
using PersonalPlannerApp.Models;

namespace PersonalPlannerApp;

public class LocalDbService
{
    private SQLiteAsyncConnection _connection;

    //-----------------------
    private async Task Init()
    {
        if (_connection == null)
        {
            _connection = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        await _connection.CreateTableAsync<ToDoItem>();
        await _connection.CreateTableAsync<PlannerItem>(); 
        await _connection.CreateTableAsync<MindItem>(); 
    }

    //------------------------
    
    public async Task<List<ToDoItem>> GetTasksAsync()
    {
        await Init();
        return await _connection.Table<ToDoItem>().ToListAsync();
    }

    public async Task SaveTaskAsync(ToDoItem item)
    {
        await Init();
        if (item.Id != 0)
            await _connection.UpdateAsync(item);
        else
            await _connection.InsertAsync(item);
    }

    public async Task DeleteTaskAsync(ToDoItem item)
    {
        await Init();
        await _connection.DeleteAsync(item);
    }
    
    // ----------------------

    public async Task<List<PlannerItem>> GetPlannerItemsForDateAsync(DateTime date) 
    { 
        await Init(); 
        var start = date.Date; 
        var end = date.Date.AddDays(1).AddTicks(-1);
        return await _connection.Table<PlannerItem>()
            .Where(i => i.Date >= start && i.Date <= end)
            .ToListAsync(); 
    }

    public async Task SavePlannerItemAsync(PlannerItem item) 
    { 
        await Init(); 
        if (item.Id != 0) 
            await _connection.UpdateAsync(item); 
        else 
            await _connection.InsertAsync(item); 
    }

    public async Task DeletePlannerItemAsync(PlannerItem item) 
    { 
        await Init(); 
        await _connection.DeleteAsync(item); 
    }

    // ------------------------

    public async Task<List<MindItem>> GetMindItemsAsync()
    {
        await Init();
        return await _connection.Table<MindItem>().ToListAsync();
    }

    public async Task SaveMindItemAsync(MindItem item)
    {
        await Init();
        if (item.Id != 0)
            await _connection.UpdateAsync(item);
        else
            await _connection.InsertAsync(item);
    }

    public async Task DeleteMindItemAsync(MindItem item)
    {
        await Init();
        await _connection.DeleteAsync(item);
    }
}