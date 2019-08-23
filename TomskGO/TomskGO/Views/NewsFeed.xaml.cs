using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TomskGO.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewsFeed : ContentPage
	{
		public NewsFeed()
		{
			InitializeComponent();
            BindingContext = Managers.CacheManager.NewsFeed;

        }
	}
}