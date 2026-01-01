namespace PersonalPlannerApp;

public static class Constants
{
    // Veritabanı dosyasının adı
    public const string DatabaseFilename = "PersonalPlanner.db3";

    // Veritabanı okuma/yazma izinleri
    public const SQLite.SQLiteOpenFlags Flags =
        SQLite.SQLiteOpenFlags.ReadWrite |
        SQLite.SQLiteOpenFlags.Create |
        SQLite.SQLiteOpenFlags.SharedCache;

    // Dosya yolunu telefonun işletim sistemine göre ayarlayan kod
    public static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
}