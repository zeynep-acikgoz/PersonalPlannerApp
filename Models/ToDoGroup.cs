using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PersonalPlannerApp.Models
{
    
    public class ToDoGroup : ObservableCollection<ToDoItem>, INotifyPropertyChanged
    {
        private readonly List<ToDoItem> _allItems;

        public string Name { get; private set; }
        
        
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
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(StateIcon))); 
                    
                    if (_isExpanded)
                    {
                        foreach (var item in _allItems) Add(item);
                    }
                    else
                    {
                        Clear();
                    }
                }
            }
        }

        // --------------------
        
        public string StateIcon => IsExpanded ? "▼" : "▶"; 

        public string DisplayName => $"{Name} ({_allItems.Count})";

        public ToDoGroup(string name, List<ToDoItem> items) : base(items)
        {
            Name = name;
            _allItems = new List<ToDoItem>(items); 
            _isExpanded = true; 
        }

        // -------------------
        
        protected override event PropertyChangedEventHandler PropertyChanged;
        
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}