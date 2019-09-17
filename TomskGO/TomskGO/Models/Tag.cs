using System.ComponentModel;

namespace TomskGO.Models
{
    class Tag : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public bool Selected { get; set; } = false;

        #region Auto-implemented
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
