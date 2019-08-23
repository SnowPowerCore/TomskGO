using System.ComponentModel;
using System.Windows.Input;
using TomskGO.Models;
using Xamarin.Forms;

namespace TomskGO.ViewModels
{
    class PostViewModel : INotifyPropertyChanged
    {
        private FeedModel _selectedItem;

        public FeedModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public PostViewModel(object feedModel)
        {
            if (feedModel is FeedModel) SelectedItem = (FeedModel)feedModel;
        }

        public ICommand NavigateBackCommand => new Command(() => Managers.TaskManager.Instance.RegisterTask(NavigateBack, true, isUIRelated: true));

        private async void NavigateBack()
        {
            await Application.Current.MainPage?.Navigation?.PopModalAsync(true);
        }

        #region Auto-implemented
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
