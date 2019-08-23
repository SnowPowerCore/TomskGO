using Newtonsoft.Json;
using System;
using TomskGO.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TomskGO.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(FeedData), "feedData")]
    public partial class Post : ContentPage
    {
        private string _feedData;
        public string FeedData
        {
            get => _feedData;
            set => _feedData = Uri.UnescapeDataString(value);
        }

        private object Item { get; set; }

        public Post(object item)
        {
            InitializeComponent();
            Item = item;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = new ViewModels.PostViewModel(Item);
        }
    }
}