using TomskGO.Core.Helpers;
using TomskGO.ViewModels;

namespace TomskGO.Views
{
    public partial class Post
    {
        public Post()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModelLocator.SetWireType(this, typeof(PostViewModel));
        }
    }
}