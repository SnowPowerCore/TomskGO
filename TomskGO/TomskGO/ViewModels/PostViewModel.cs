using Stormlion.PhotoBrowser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TomskGO.Core.ViewModels;
using TomskGO.Models.API;
using Xamarin.Forms;

namespace TomskGO.ViewModels
{
    class PostViewModel : BaseViewModel
    {
        private NewsModel _selectedItem;

        public NewsModel SelectedItem
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
            if (feedModel is NewsModel) SelectedItem = (NewsModel)feedModel;
        }

        public ICommand NavigateBackCommand => new Command(() => 
            Managers.TaskManager.Instance.RegisterTask(async () => await NavigateBackAsync(), true, isUIRelated: true));
        public ICommand OpenPhotoCommand => new Command<NewsAttachment.Photo>((p) => 
            Managers.TaskManager.Instance.RegisterTask(() => OpenPhoto(p), true, isUIRelated: true));
        public ICommand OpenFilterCommand => new Command<string>((t) =>
            Managers.TaskManager.Instance.RegisterTask(async () => await OpenFilterAsync(t), true, isUIRelated: true));

        private void OpenPhoto(NewsAttachment.Photo photo)
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
    }
}