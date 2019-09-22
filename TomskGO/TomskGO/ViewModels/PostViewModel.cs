using Stormlion.PhotoBrowser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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

        public ICommand NavigateBackCommand => new Command(() => 
            Managers.TaskManager.Instance.RegisterTask(async () => await NavigateBackAsync(), true, isUIRelated: true));
        public ICommand OpenPhotoCommand => new Command<FeedModel.Attachment.Photo>((p) => 
            Managers.TaskManager.Instance.RegisterTask(() => OpenPhoto(p), true, isUIRelated: true));
        public ICommand OpenFilterCommand => new Command<string>((t) =>
            Managers.TaskManager.Instance.RegisterTask(async () => await OpenFilterAsync(t), true, isUIRelated: true));

        private void OpenPhoto(FeedModel.Attachment.Photo photo)
        {
            var browser = new PhotoBrowser
            {
                Photos = new List<Photo>(SelectedItem.Attachments.Photos
                    .Select(x => new Photo { URL = x.ImageSource })),
                StartIndex = SelectedItem.Attachments.Photos.IndexOf(SelectedItem.Attachments.Photos.FirstOrDefault(x => x.ImageSource == photo.ImageSource))
            };
            browser.Show();
        }

        private async Task OpenFilterAsync(string tagName)
        {
            var modified = Uri.EscapeDataString(tagName);
            await Shell.Current.GoToAsync("//filter?tagName=" + modified);
        }

        private async Task NavigateBackAsync()
        {
            await Shell.Current.Navigation.PopAsync(true);
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