using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TomskGO.Managers;
using TomskGO.Models;
using TomskGO.Providers;
using Xamarin.Forms;

namespace TomskGO.ViewModels
{
    class NewsFeedViewModel : INotifyPropertyChanged
    {
        private List<AbstractFeedProvider> _providers;
        private ObservableRangeCollection<FeedModel> _posts;
        private ObservableRangeCollection<FeedModel> _filteredPosts;
        private ObservableRangeCollection<Tag> _tags;

        public List<AbstractFeedProvider> Providers
        {
            get => _providers;
            set
            {
                _providers = value;
                OnPropertyChanged();
            }
        }

        public ObservableRangeCollection<FeedModel> Posts
        {
            get => _posts;
            set
            {
                if(value.Count > 0)
                _posts = value;
                OnPropertyChanged();
            }
        }
        public ObservableRangeCollection<FeedModel> FilteredPosts
        {
            get => _filteredPosts;
            set
            {
                if (value.Count > 0)
                _filteredPosts = value;
                OnPropertyChanged();
            }
        }

        public ObservableRangeCollection<Tag> Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                OnPropertyChanged();
            }
        }

        public NewsFeedViewModel()
        {
            Providers = new List<AbstractFeedProvider>
            {
                new VKProvider(new VKRequestModel
                {
                    owner_id = 66471096,
                    extended = VKRequestModel.ExtendedFlag.no,
                    filter = VKRequestModel.PostType.all
                })
            };
            RefreshFeedCommand?.Execute(null);
        }

        public ICommand RefreshFeedCommand =>
            new Command(() => TaskManager.Instance.RegisterTask(async () => await RefreshFeedAsync(), true));
        public ICommand NavigatePostCommand =>
            new Command<FeedModel>(f => TaskManager.Instance.RegisterTask(() => NavigatePost(f), true, isUIRelated: true));
        public ICommand FilterPostsCommand =>
            new Command<string>(t => TaskManager.Instance.RegisterTask(() => FilterPosts(t), true));
        public ICommand ChangeTagSelectionCommand =>
            new Command<Tag>(t => TaskManager.Instance.RegisterTask(() => ChangeTagSelection(t), true));

        private async Task RefreshFeedAsync()
        {
            foreach (var provider in Providers)
            {
                var data = await provider.ProvideData();
                Posts = new ObservableRangeCollection<FeedModel>(data);
            }
            Tags = new ObservableRangeCollection<Tag>(Posts.SelectMany(x => x.Tags.Select(t => new Tag { Name = t })).Distinct());
            FilteredPosts = Posts;
        }

        private void FilterPosts(string t)
        {
            if (Posts == null) return;
            FilteredPosts = new ObservableRangeCollection<FeedModel>(Posts
                .Where(x => x.Tags.Any(tag => tag == t)));
            Tags.FirstOrDefault(x => x.Name == t).Selected = true;
        }

        private void ChangeTagSelection(Tag t)
        {
            t.Selected = !t.Selected;
            FilteredPosts = new ObservableRangeCollection<FeedModel>(Posts
                .TakeWhile(p => p.Tags.Any(tag => Tags.Any(x => x.Selected && x.Name == tag))));
        }

        private async void NavigatePost(FeedModel item)
        {
            var serialized = JsonConvert.SerializeObject(item);
            var modified = Uri.EscapeDataString(serialized);
            await Shell.Current.GoToAsync("//news/post?feedData="+modified);
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