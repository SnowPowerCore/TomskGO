using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Platform;
using Microsoft.Extensions.DependencyInjection;
using TomskGO.Core;
using TomskGO.Core.Interfaces;
using TomskGO.Droid.Implementations;
using Xamarin;
using AGlide = Android.Glide;

namespace TomskGO.Droid
{
    [Activity(Label = "Tomsk GO!", Icon = "@drawable/icon", Theme = "@style/MainTheme",
        MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            AGlide.Forms.Init(this);
            XamEffects.Droid.Effects.Init();
            PanCardView.Droid.CardsViewRenderer.Preserve();
            XF.Material.Droid.Material.Init(this, savedInstanceState);
            Stormlion.PhotoBrowser.Droid.Platform.Init(this);
            FormsGoogleMaps.Init(this, savedInstanceState, new Xamarin.Forms.GoogleMaps.Android.PlatformConfig
            {
                BitmapDescriptorFactory = new CachingNativeBitmapDescriptorFactory()
            });
            CachedImageRenderer.Init(true);
            _ = typeof(FFImageLoading.Svg.Forms.SvgCachedImage);
            LoadApplication(Startup.Init(ConfigureServices));
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(ILocalizeService), typeof(LocalizeService));
            services.AddSingleton(typeof(IAppQuit), typeof(AppQuit));
            services.AddSingleton(typeof(IToast), typeof(Toast));
        }
    }
}