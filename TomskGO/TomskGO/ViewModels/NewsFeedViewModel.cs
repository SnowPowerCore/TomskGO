using AsyncAwaitBestPractices.MVVM;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TomskGO.Core.ViewModels;
using TomskGO.Models.API;
using TomskGO.Models.Utils;
using Xamarin.Forms;

namespace TomskGO.ViewModels
{
    class NewsFeedViewModel : BaseViewModel
    {
        private ObservableRangeCollection<NewsModel> _posts;
        private ObservableRangeCollection<NewsModel> _filteredPosts;
        private ObservableRangeCollection<NewsTag> _tags;
        private ObservableRangeCollection<NewsTag> _selectedTags;
        
        public ObservableRangeCollection<NewsModel> Posts
        {
            get => _posts;
            set
            {
                if(value.Count > 0)
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

        public NewsFeedViewModel()
        {
            RefreshFeedCommand?.Execute(null);
        }

        public ICommand RefreshFeedCommand =>
            new AsyncCommand(RefreshFeedAsync);
        public ICommand NavigatePostCommand =>
            new Command<NewsModel>(NavigatePost);
        public ICommand FilterPostsCommand =>
            new Command<string>(FilterPosts);
        public ICommand ChangeTagSelectionCommand =>
            new Command<NewsTag>(ChangeTagSelection);

        private async Task RefreshFeedAsync()
        {

        }

        private void FilterPosts(string t)
        {
            if (Posts == null) return;
            FilteredPosts = new ObservableRangeCollection<NewsModel>(Posts
                .Where(x => x.Tags.Any(tag => tag.Name == t)));
            var searchedTag = Tags.FirstOrDefault(x => x.Name == t);
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

        private async void NavigatePost(NewsModel item)
        {
            var serialized = JsonConvert.SerializeObject(item);
            var modified = Uri.EscapeDataString(serialized);
            await Shell.Current.GoToAsync("post?feedData="+modified);
        }
    }
}