using TomskGO.Core.ViewModels.News;

namespace TomskGO.Core.Views
{
    public partial class Filter
    {
        public NewsFeedViewModel NewsFeedViewModel =>
            (NewsFeedViewModel)BindingContext;

        public Filter() =>
            InitializeComponent();

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NewsFeedViewModel.FilterPostsCommand?.Execute(null);
        }
    }
}