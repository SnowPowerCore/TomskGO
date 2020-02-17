using TomskGO.Core.ViewModels.News;

namespace TomskGO.Core.Views
{
    public partial class Post
    {
        private PostViewModel PostViewModel =>
            (PostViewModel)BindingContext;

        public Post() =>
            InitializeComponent();

        protected override void OnAppearing()
        {
            base.OnAppearing();
            PostViewModel.UpdateCurrentPostCommand?.Execute(null);
        }
    }
}