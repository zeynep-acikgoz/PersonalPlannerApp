using SQLite; // Veritabanı özellikleri için
using Microsoft.Maui.Graphics;

namespace PersonalPlannerApp.Models;

// Bu sınıfın bir Veritabanı Tablosu olduğunu belirtiyoruz
[Table("todo_items")]
public class ToDoItem
{
    // PrimaryKey: Her görevin benzersiz bir numarası olacak
    // AutoIncrement: Numarayı biz vermicez, sistem 1-2-3 diye artıracak
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Title { get; set; }

    // Tamamlanma Durumu
    public bool IsCompleted { get; set; }

    // Son Tarih (Boş olabilir)
    public DateTime? DueDate { get; set; }

    // --- YENİ EKLENENLER (Senin İsteklerin) ---

    // Kategori (Örn: "Okul", "İş", "Kişisel")
    // Bunu ekranda Mavi/Mor nokta olarak göstereceğiz
    public string Category { get; set; } 

    // Öncelik Seviyesi (0: Low, 1: Medium, 2: High)
    // Listeyi buna göre gruplayacağız
    public int PriorityLevel { get; set; } 
    
    // UI Yardımcısı: Ekranda görünecek renk (Veritabanına kaydetmeye gerek yok)
    [Ignore] 
    public Color DisplayColor 
    {
        get
        {
            // Basit bir mantık: Kategoriye göre renk döndür
            if (Category == "School") return Colors.Blue;
            if (Category == "Work") return Colors.Purple;
            return Colors.Pink; // Personal ve diğerleri
        }
    }
    
    public Color CategoryColor
    {
        get
        {
            // Kategori ismin neyse ona göre renk seçiyoruz
            return Category switch
            {
                "School" => Colors.CornflowerBlue, // Okul -> Mavi
                "Work" => Colors.MediumPurple,     // İş -> Mor
                "Personal" => Colors.HotPink,      // Kişisel -> Pembe
                _ => Colors.Gray                   // Tanımsızsa -> Gri
            };
        }
    }
}