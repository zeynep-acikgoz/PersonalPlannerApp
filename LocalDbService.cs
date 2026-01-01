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

        // Bağlantıyı kur
        _connection = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

        // Tabloları oluştur (Eğer yoksa)
        // Şimdilik sadece ToDoItem tablosunu oluşturuyoruz
        await _connection.CreateTableAsync<ToDoItem>();
    }

    // --- TO DO İŞLEMLERİ ---

    // 1. Tüm Görevleri Getir
    public async Task<List<ToDoItem>> GetTasksAsync()
    {
        await Init();
        return await _connection.Table<ToDoItem>().ToListAsync();
    }

    // 2. Tek Bir Görevi Getir (ID ile)
    public async Task<ToDoItem> GetTaskByIdAsync(int id)
    {
        await Init();
        return await _connection.Table<ToDoItem>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }

    // 3. Ekle veya Güncelle (Varsa güncelle, yoksa ekle)
    public async Task SaveTaskAsync(ToDoItem item)
    {
        await Init();
        if (item.Id != 0)
            await _connection.UpdateAsync(item);
        else
            await _connection.InsertAsync(item);
    }

    // 4. Sil
    public async Task DeleteTaskAsync(ToDoItem item)
    {
        await Init();
        await _connection.DeleteAsync(item);
    }
}