using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Platform;
using Sharpnado.Presentation.Forms.Droid;

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
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            XamEffects.Droid.Effects.Init();
            XF.Material.Droid.Material.Init(this, savedInstanceState);
            CachedImageRenderer.Init(true);
            SharpnadoInitializer.Initialize();
            _ = typeof(FFImageLoading.Svg.Forms.SvgCachedImage);
            LoadApplication(new App());
        }
    }
}