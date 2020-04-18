using AsyncAwaitBestPractices.MVVM;
using Stormlion.PhotoBrowser;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TomskGO.Core.Services.Tomsk.News;
using TomskGO.Core.Services.Utils.Navigation;
using TomskGO.Models.API;
using Xamarin.Essentials;
using Xamarin.Forms;
using Photo = TomskGO.Models.API.Photo;
using PhotoView = Stormlion.PhotoBrowser.Photo;

namespace TomskGO.Core.ViewModels.News
{
    public class PostViewModel : BaseViewModel
    {
        private NewsContext _news;
        private INavigationService _navigation;

        private IAsyncCommand _navigateBackCommand;
        private IAsyncCommand<string> _openFilterCommand;
        private IAsyncCommand<string> _openUrlCommand;
        private ICommand _updateCurrentPostCommand;
        private ICommand _openPhotoCommand;

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

        public PostViewModel(NewsContext news,
                             INavigationService navigation)
        {
            _news = news;
            _navigation = navigation;
        }

        public IAsyncCommand NavigateBackCommand => _navigateBackCommand
            ?? (_navigateBackCommand = new AsyncCommand(NavigateBackAsync));

        public IAsyncCommand<string> OpenFilterCommand => _openFilterCommand
            ?? (_openFilterCommand = new AsyncCommand<string>(OpenFilterAsync));

        public IAsyncCommand<string> OpenUrlCommand => _openUrlCommand
            ?? (_openUrlCommand = new AsyncCommand<string>(OpenUrlAsync));

        public ICommand UpdateCurrentPostCommand => _updateCurrentPostCommand
            ?? (_updateCurrentPostCommand = new Command(UpdateCurrentPost));

        public ICommand OpenPhotoCommand => _openPhotoCommand
            ?? (_openPhotoCommand = new Command<Photo>(OpenPhoto));

        private void UpdateCurrentPost() =>
            SelectedItem = _news.NewsRepository.GetNewsItem();

        private void OpenPhoto(Photo photo)
        {
            var browser = new PhotoBrowser
            {
                Photos = new List<PhotoView>(SelectedItem.Attachments.Photos
                    .Select(x => new PhotoView { URL = x.ImageSource })),
                StartIndex = SelectedItem.Attachments.Photos.IndexOf(
                    SelectedItem.Attachments.Photos
                        .FirstOrDefault(x => x.ImageSource == photo.ImageSource))
            };
            browser.Show();
        }

        private Task OpenUrlAsync(string url) =>
            Launcher.OpenAsync(url);

        private Task OpenFilterAsync(string tagName)
        {
            _news.NewsRepository.SetTagName(tagName);
            return _navigation.NavigateToPageAsync("//filter");
        }

        private Task NavigateBackAsync() =>
            _navigation.NavigateBackAsync();
    }
}