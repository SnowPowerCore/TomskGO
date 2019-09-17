namespace TomskGO.Views
{
    public partial class NewsFeed
	{
		public NewsFeed()
		{
			InitializeComponent();
            BindingContext = Managers.CacheManager.NewsFeed;
        }
	}
}