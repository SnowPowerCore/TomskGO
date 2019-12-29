using System.ComponentModel;

namespace TomskGO.Core.ViewModels
{
    class BaseViewModel : INotifyPropertyChanged
    {
        #region Auto-implemented
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
