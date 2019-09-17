using System;
using Xamarin.Forms;

namespace TomskGO.Views
{
    [QueryProperty(nameof(TagName), "tagName")]
    public partial class Filter
    {
        private string _tagName;
        public string TagName
        {
            get => _tagName;
            set => _tagName = Uri.UnescapeDataString(value);
        }

        public Filter()
        {
            InitializeComponent();
            BindingContext = Managers.CacheManager.NewsFeed;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!string.IsNullOrEmpty(TagName))
                Managers.CacheManager.NewsFeed.FilterPostsCommand?.Execute(TagName);
        }
    }
}