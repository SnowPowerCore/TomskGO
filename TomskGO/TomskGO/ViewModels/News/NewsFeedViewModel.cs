using AsyncAwaitBestPractices.MVVM;
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
        private readonly INewsService _news;
        private readonly INavigationService _navigation;

        private ObservableRangeCollection<NewsModel> _posts;
        private ObservableRangeCollection<NewsModel> _filteredPosts;
        private ObservableRangeCollection<NewsTag> _tags;
        private ObservableRangeCollection<NewsTag> _selectedTags;
        #endregion

        #region Properties
        public ObservableRangeCollection<NewsModel> Posts
        {
            get => _posts;
            set
            {
                if (value.Count > 0)
                    _posts = value;
                OnPropertyChanged();
            }
        }

        public ObservableRangeCollection<NewsModel> FilteredPosts
        {
            get => _filteredPosts;
            set
            {
                if (value.Count > 0)
                    _filteredPosts = value;
                OnPropertyChanged();
            }
        }

        public ObservableRangeCollection<NewsTag> Tags
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
        public IAsyncCommand RefreshFeedCommand =>
            new AsyncCommand(RefreshFeedAsync);

        public IAsyncCommand<NewsModel> NavigatePostCommand =>
            new AsyncCommand<NewsModel>(NavigatePostAsync);

        public ICommand FilterPostsCommand =>
            new Command(FilterPosts);

        public ICommand ChangeTagSelectionCommand =>
            new Command<NewsTag>(ChangeTagSelection);
        #endregion

        #region Constructor
        public NewsFeedViewModel(INewsService news,
                                 INavigationService navigation)
        {
            _news = news;
            _navigation = navigation;

            RefreshFeedCommand?.ExecuteAsync();
        }
        #endregion

        #region Methods
        private Task RefreshFeedAsync() =>
            _news.GetAllNewsAsync()
                .ContinueWith(t =>
                {
                    Posts = new ObservableRangeCollection<NewsModel>(t.Result);
                    FilteredPosts = Posts;
                    Tags = new ObservableRangeCollection<NewsTag>(Posts
                        .SelectMany(t => t.Tags)
                        .Distinct());
                    SelectedTags = new ObservableRangeCollection<NewsTag>();
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

        private void FilterPosts()
        {
            if (string.IsNullOrEmpty(_news.SelectedTagName)) return;
            var tagName = _news.SelectedTagName;
            FilteredPosts = new ObservableRangeCollection<NewsModel>(Posts
                .Where(x => x.Tags.Any(tag => tag.Name == tagName)));
            var searchedTag = Tags.FirstOrDefault(tag => tag.Name == tagName);
            searchedTag.Selected = true;
            SelectedTags = new ObservableRangeCollection<NewsTag> { searchedTag };
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
                FilteredPosts = Posts;
                return;
            }

            FilteredPosts = new ObservableRangeCollection<NewsModel>(Posts
                .Where(x => x.Tags.Any(tag => SelectedTags.Any(s => s.Name == tag.Name))));
        }

        private Task NavigatePostAsync(NewsModel item)
        {
            _news.SelectedPost = item;
            return _navigation.NavigateToPageAsync("post");
        }
        #endregion
    }
}