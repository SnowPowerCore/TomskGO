using Android.App;
using TomskGO.Core.Interfaces;

namespace TomskGO.Android.Implementations
{
    public class AppQuit : IAppQuit
    {
        public void Quit()
        {
            ((Activity)Xamarin.Forms.Forms.Context).FinishAffinity();
        }
    }
}