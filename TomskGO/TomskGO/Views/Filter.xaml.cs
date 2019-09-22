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
            set
            {
                var data = Uri.UnescapeDataString(value);
                if (!string.IsNullOrEmpty(data))
                    Managers.CacheManager.NewsFeed.FilterPostsCommand?.Execute(data);
                _tagName = data;
            }
        }

        public Filter()
        {
            InitializeComponent();
            BindingContext = Managers.CacheManager.NewsFeed;
        }
    }
}