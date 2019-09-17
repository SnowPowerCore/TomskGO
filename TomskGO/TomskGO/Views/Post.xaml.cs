using Newtonsoft.Json;
using System;
using TomskGO.Models;
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

        private FeedModel SelectedItem => JsonConvert.DeserializeObject<FeedModel>(FeedData);

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