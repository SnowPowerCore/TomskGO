using Android.App;
using TomskGO.Core.Interfaces;

namespace TomskGO.Android.Implementations
{
    internal class AppQuit : IAppQuit
    {
        public void Quit()
        {
            ((Activity)Xamarin.Forms.Forms.Context).FinishAffinity();
        }
    }
}