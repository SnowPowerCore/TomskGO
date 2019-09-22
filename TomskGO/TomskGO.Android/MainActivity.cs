using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Platform;
using AGlide = Android.Glide;

namespace TomskGO.Droid
{
    [Activity(Label = "Tomsk GO!", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            AGlide.Forms.Init();
            XamEffects.Droid.Effects.Init();
            PanCardView.Droid.CardsViewRenderer.Preserve();
            XF.Material.Droid.Material.Init(this, savedInstanceState);
            Stormlion.PhotoBrowser.Droid.Platform.Init(this);
            CachedImageRenderer.Init(true);
            _ = typeof(FFImageLoading.Svg.Forms.SvgCachedImage);
            LoadApplication(new App());
        }
    }
}