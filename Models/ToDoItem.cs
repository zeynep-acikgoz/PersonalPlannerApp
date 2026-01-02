using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;
using SQLite;

namespace PersonalPlannerApp.Models
{
    
    public class ToDoItem : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; }
        
       
        private bool _isCompleted;
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged(); 
                }
            }
        }
        
        // -------------------------

        
        
        public DateTime? DueDate { get; set; }
        public string Category { get; set; }
        public int PriorityLevel { get; set; }

        
        [Ignore] 
        public Color CategoryColor
        {
            get
            {
                return Category switch
                {
                    "School" => Colors.CornflowerBlue,
                    "Work" => Colors.MediumPurple,
                    "Personal" => Colors.HotPink,
                    _ => Colors.Gray
                };
            }
        }

        [Ignore]
        public Color PriorityColor
        {
            get
            {
                return PriorityLevel switch
                {
                    2 => Colors.OrangeRed,
                    1 => Colors.Orange,
                    0 => Colors.SeaGreen,
                    _ => Colors.Gray
                };
            }
        }

        // ----------
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        
        // --------------
        
        private bool _isPlanned;
        public bool IsPlanned
        {
            get => _isPlanned;
            set
            {
                if (_isPlanned != value)
                {
                    _isPlanned = value;
                    OnPropertyChanged(); 
                }
            }
        }
        
        
    }
}