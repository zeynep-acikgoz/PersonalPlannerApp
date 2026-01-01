using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PersonalPlannerApp.Models
{
    // Bu sınıf hem bir listedir hem de ekranı dinleyen bir yapıdır (INotifyPropertyChanged)
    public class ToDoGroup : ObservableCollection<ToDoItem>, INotifyPropertyChanged
    {
        // Kapandığında verileri kaybetmemek için yedek depo
        private readonly List<ToDoItem> _allItems;

        public string Name { get; private set; }

        // --- AÇMA / KAPAMA MANTIĞI ---
        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsExpanded)));
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(StateIcon))); // İkonu değiştir
                    
                    if (_isExpanded)
                    {
                        // Açıldıysa: Yedekteki her şeyi listeye geri ekle
                        foreach (var item in _allItems) Add(item);
                    }
                    else
                    {
                        // Kapandıysa: Listeyi temizle (ama _allItems içinde yedeği var)
                        Clear();
                    }
                }
            }
        }

        // --- OK İKONU ---
        // Açıksa Aşağı Ok, Kapalıysa Yukarı/Sağ Ok
        public string StateIcon => IsExpanded ? "▼" : "▶"; 

        // Başlıkta kaç tane olduğunu göstermek için (Örn: High Priority (3))
        public string DisplayName => $"{Name} ({_allItems.Count})";

        // --- KURUCU METOT (CONSTRUCTOR) ---
        public ToDoGroup(string name, List<ToDoItem> items) : base(items)
        {
            Name = name;
            _allItems = new List<ToDoItem>(items); // Gelen listeyi yedekle
            _isExpanded = true; // Başlangıçta hepsi AÇIK gelsin
        }

        // --- GEREKLİ OLAY YÖNETİCİSİ ---
        protected override event PropertyChangedEventHandler PropertyChanged;
        
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}