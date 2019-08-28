using FFImageLoading.Forms.Platform;
using Foundation;
using UIKit;

namespace TomskGO.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            Xamarin.Forms.Forms.Init();
            FFImageLoading.FormsHandler.Init();
            XamEffects.iOS.Effects.Init();
            PanCardView.iOS.CardsViewRenderer.Preserve();
            XF.Material.iOS.Material.Init();
            CachedImageRenderer.Init();
            _ = typeof(FFImageLoading.Svg.Forms.SvgCachedImage);
            LoadApplication(new App());
            Stormlion.PhotoBrowser.iOS.Platform.Init();
            return base.FinishedLaunching(app, options);
        }
    }
}
