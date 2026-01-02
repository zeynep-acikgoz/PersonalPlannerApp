using SQLite;
using PersonalPlannerApp.Models;

namespace PersonalPlannerApp;

public class LocalDbService
{
    // Veritabanı bağlantısı
    private SQLiteAsyncConnection _connection;

    // Veritabanını başlatan fonksiyon
    private async Task Init()
    {
        if (_connection != null)
            return;

        // Bağlantı kurulumu
        _connection = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

        
        await _connection.CreateTableAsync<ToDoItem>();
        await _connection.CreateTableAsync<PlannerItem>(); 
    }

    // -------------- TO DO
    
    public async Task<List<ToDoItem>> GetTasksAsync()
    {
        await Init();
        return await _connection.Table<ToDoItem>().ToListAsync();
    }

    public async Task<ToDoItem> GetTaskByIdAsync(int id)
    {
        await Init();
        return await _connection.Table<ToDoItem>().Where(i => i.Id == id).FirstOrDefaultAsync();
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
    
    
    
    // --------------- PLANNER 

    public async Task<List<PlannerItem>> GetPlannerItemsForDateAsync(DateTime date)
    {
        await Init();
        
        
        var startOfDay = date.Date;
        var endOfDay = date.Date.AddDays(1).AddTicks(-1);

        return await _connection.Table<PlannerItem>()
            .Where(i => i.Date >= startOfDay && i.Date <= endOfDay)
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
    
}