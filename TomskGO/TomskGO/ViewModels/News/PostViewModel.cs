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

namespace TomskGO.Core.ViewModels.News
{
    public class PostViewModel : BaseViewModel
    {
        private INewsService _news;
        private INavigationService _navigation;

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

        public PostViewModel(INewsService news,
                             INavigationService navigation)
        {
            _news = news;
            _navigation = navigation;
        }

        public ICommand UpdateCurrentPostCommand =>
            new Command(UpdateCurrentPost);

        public IAsyncCommand NavigateBackCommand =>
            new AsyncCommand(NavigateBackAsync);

        public IAsyncCommand<string> OpenFilterCommand =>
            new AsyncCommand<string>(OpenFilterAsync);

        public IAsyncCommand<string> OpenUrlCommand =>
            new AsyncCommand<string>(OpenUrlAsync);

        public ICommand OpenPhotoCommand =>
            new Command<NewsAttachment.Photo>(OpenPhoto);

        private void UpdateCurrentPost() =>
            SelectedItem = _news.SelectedPost;

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

        private Task OpenUrlAsync(string url) =>
            Launcher.OpenAsync(url);

        private Task OpenFilterAsync(string tagName)
        {
            _news.SelectedTagName = tagName;
            return _navigation.NavigateToPageAsync("//filter");
        }

        private Task NavigateBackAsync() =>
            _navigation.NavigateBackAsync();
    }
}