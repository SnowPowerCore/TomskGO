using TomskGO.Core.Helpers;
using TomskGO.ViewModels;

namespace TomskGO.Views
{
    public partial class NewsFeed
	{
		public NewsFeed()
		{
			InitializeComponent();
			ViewModelLocator.SetWireType(this, typeof(NewsFeedViewModel));
        }
	}
}