using Newtonsoft.Json;
using System;
using TomskGO.Models.API;
using Xamarin.Forms;

namespace TomskGO.Views
{
    [QueryProperty(nameof(FeedData), "feedData")]
    public partial class Post
    {
        private string _feedData;
        public string FeedData
        {
            get => _feedData;
            set => _feedData = Uri.UnescapeDataString(value);
        }

        private NewsModel SelectedItem => JsonConvert.DeserializeObject<NewsModel>(FeedData);

        public Post()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = new ViewModels.PostViewModel(SelectedItem);
        }
    }
}