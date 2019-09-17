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
        private bool isBusy = false;

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
                if(value.Count > 0) IsBusy = true;
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

        public bool IsBusy
        {
            get => isBusy;
            private set
            {
                isBusy = value;
                OnPropertyChanged();
            }
        }

        public NewsFeedViewModel()
        {
            Posts = new ObservableRangeCollection<FeedModel>();
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

        private async Task RefreshFeedAsync()
        {
            foreach(var provider in Providers)
            {
                Posts = new ObservableRangeCollection<FeedModel>(Posts.Concat(await provider.ProvideData()));
            }
            Tags = new ObservableRangeCollection<Tag>(Posts.SelectMany(x => x.Tags.Select(t => new Tag { Name = t })).Distinct());
            FilteredPosts = Posts;
        }

        private void FilterPosts(string t)
        {
            if (Posts == null) return;
            FilteredPosts = new ObservableRangeCollection<FeedModel>(Posts.Where(x => x.Tags.Any(tag => tag == t)));
            Tags.FirstOrDefault(x => x.Name == t).Selected = true;
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
