using AsyncAwaitBestPractices.MVVM;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TomskGO.Core.Services.Tomsk.News;
using TomskGO.Core.Services.Utils.Navigation;
using TomskGO.Models.API;
using TomskGO.Models.Utils;
using Xamarin.Forms;

namespace TomskGO.Core.ViewModels.News
{
    public class NewsFeedViewModel : BaseViewModel
    {
        #region Fields
        private readonly NewsContext _news;
        private readonly INavigationService _navigation;

        private IAsyncCommand _refreshFeedCommand;
        private IAsyncCommand<NewsModel> _navigatePostCommand;
        private ICommand _filterPostsCommand;
        private ICommand _changeTagSelectionCommand;

        private List<NewsModel> _posts = new List<NewsModel>();
        private ObservableRangeCollection<NewsModel> _filteredPosts =
            new ObservableRangeCollection<NewsModel>();
        private List<NewsTag> _tags = new List<NewsTag>();
        private ObservableRangeCollection<NewsTag> _selectedTags =
            new ObservableRangeCollection<NewsTag>();
        #endregion

        #region Properties
        public List<NewsModel> Posts
        {
            get => _posts;
            set
            {
                _posts = value;
                OnPropertyChanged();
            }
        }

        public ObservableRangeCollection<NewsModel> FilteredPosts
        {
            get => _filteredPosts;
            set
            {
                _filteredPosts = value;
                OnPropertyChanged();
            }
        }

        public List<NewsTag> Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                OnPropertyChanged();
            }
        }

        public ObservableRangeCollection<NewsTag> SelectedTags
        {
            get => _selectedTags;
            set
            {
                _selectedTags = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public IAsyncCommand RefreshFeedCommand => _refreshFeedCommand
            ?? (_refreshFeedCommand = new AsyncCommand(RefreshFeedAsync));

        public IAsyncCommand<NewsModel> NavigatePostCommand => _navigatePostCommand
            ?? (_navigatePostCommand = new AsyncCommand<NewsModel>(NavigatePostAsync));

        public ICommand FilterPostsCommand => _filterPostsCommand
            ?? (_filterPostsCommand = new Command(FilterPosts));

        public ICommand ChangeTagSelectionCommand => _changeTagSelectionCommand
            ?? (_changeTagSelectionCommand = new Command<NewsTag>(ChangeTagSelection));
        #endregion

        #region Constructor
        public NewsFeedViewModel(NewsContext news,
                                 INavigationService navigation)
        {
            _news = news;
            _navigation = navigation;

            RefreshFeedCommand?.Execute(null);
        }
        #endregion

        #region Methods
        private async Task RefreshFeedAsync()
        {
            SelectedTags.Clear();
            Tags.Clear();
            FilteredPosts.Clear();
            Posts.Clear();

            var result = await _news.NewsService.GetAllNewsAsync().ConfigureAwait(false);

            await Device.InvokeOnMainThreadAsync(() =>
            {
                Posts = result;
                FilteredPosts.AddRange(Posts);
                Tags = Posts
                    .SelectMany(t => t.Tags.Select(x => x.Name))
                    .Distinct()
                    .Select(x => new NewsTag { Name = x })
                    .ToList();
            });
        }

        private void FilterPosts()
        {
            if (string.IsNullOrEmpty(_news.NewsRepository.GetTagName())) return;

            var tagName = _news.NewsRepository.GetTagName();
            FilteredPosts = new ObservableRangeCollection<NewsModel>(Posts
                .Where(x => x.Tags.Any(tag => tag.Name == tagName)));
            var searchedTag = Tags.FirstOrDefault(tag => tag.Name == tagName);
            searchedTag.Selected = true;
            SelectedTags.Add(searchedTag);
        }

        private void ChangeTagSelection(NewsTag t)
        {
            if (t.Selected)
            {
                if (SelectedTags.Contains(t))
                {
                    SelectedTags.Remove(t);
                    t.Selected = false;
                }
            }
            else
            {
                SelectedTags.Add(t);
                t.Selected = true;
            }

            if (SelectedTags.Count == 0)
            {
                FilteredPosts = new ObservableRangeCollection<NewsModel>(Posts);
                return;
            }

            FilteredPosts = new ObservableRangeCollection<NewsModel>(Posts
                .Where(x => x.Tags.Any(tag => SelectedTags.Any(s => s.Name == tag.Name))));
        }

        private Task NavigatePostAsync(NewsModel item)
        {
            _news.NewsRepository.SetNewsItem(item);
            return _navigation.NavigateToPageAsync("post");
        }
        #endregion
    }
}