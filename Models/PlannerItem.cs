using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PersonalPlannerApp.Models
{
    public class PlannerItem : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; }       
        public DateTime Date { get; set; }      
        public TimeSpan StartTime { get; set; } 
        public TimeSpan EndTime { get; set; }   

        // -------------
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

        public int? LinkedToDoId { get; set; }

     
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}