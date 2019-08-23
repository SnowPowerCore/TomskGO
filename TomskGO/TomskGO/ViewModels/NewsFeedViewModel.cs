using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TomskGO.Managers;
using TomskGO.Models;
using TomskGO.Providers;
using TomskGO.Views;
using Xamarin.Forms;

namespace TomskGO.ViewModels
{
    class NewsFeedViewModel : INotifyPropertyChanged
    {
        private List<AbstractFeedProvider> _providers;
        private ObservableCollection<FeedModel> _posts;
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

        public ObservableCollection<FeedModel> Posts
        {
            get => _posts;
            set
            {
                if(value.Count > 0) IsBusy = true;
                _posts = value;
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
            Posts = new ObservableCollection<FeedModel>();
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

        public ICommand RefreshFeedCommand => new Command(() => TaskManager.Instance.RegisterTask(RefreshFeed, true));
        public ICommand NavigatePostCommand => new Command<FeedModel>((f) => TaskManager.Instance.RegisterTask(() => NavigatePost(f), true, isUIRelated: true));

        private void RefreshFeed()
        {
            Parallel.ForEach(Providers, async x =>
            {
                Posts = new ObservableCollection<FeedModel>(Posts.Concat(await x.ProvideData()));
            });
        }

        private async void NavigatePost(FeedModel item)
        {
            await Application.Current.MainPage?.Navigation?.PushModalAsync(new Xamarin.Forms.NavigationPage(new Post(item)), true);
            //await Shell.Current.GoToAsync("//news/post?feedData="+JsonConvert.SerializeObject(item));
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
