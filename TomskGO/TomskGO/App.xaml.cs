using System;
using TomskGO.Core.Services.Utils.Analytics;
using TomskGO.Core.Services.Utils.Language;
using TomskGO.Core.Services.Utils.Navigation;
using TomskGO.Core.Services.Utils.Theme;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Application = Xamarin.Forms.Application;

namespace TomskGO
{
    public partial class App : Application
    {
        #region Fields
        private IThemeService _theme;
        private ILanguageService _language;
        private INavigationService _navigation;
        private IAnalyticsService _analytics;
        #endregion

        #region Properties
        public static IServiceProvider Services { get; set; }
        #endregion

        #region Constructor
        public App()
        {
            InitializeComponent();
            Current
                .On<Android>()
                .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

            InitApp();
        }
        #endregion

        #region Methods
        private void InitApp()
        {
            _theme = (IThemeService)Services.GetService(typeof(IThemeService));
            _language = (ILanguageService)Services.GetService(typeof(ILanguageService));
            _navigation = (INavigationService)Services.GetService(typeof(INavigationService));
            _analytics = (IAnalyticsService)Services.GetService(typeof(IAnalyticsService));
        }

        protected override void OnStart()
        {
            base.OnStart();

            _theme.DetermineAndLoadTheme();
            _language.DetermineAndSetLanguage();
            _navigation.DetermineAndSetMainPage();
            _analytics.TrackEvent("App started.");
        }
        #endregion
    }
}