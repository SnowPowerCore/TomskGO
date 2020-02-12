using System;
using TomskGO.Core.Services.Utils.Language;
using TomskGO.Core.Services.Utils.Navigation;
using TomskGO.Core.Services.Utils.Theme;
using Xamarin.Forms;

namespace TomskGO
{
    public partial class App : Application
    {
        #region Fields
        private IThemeService _theme;
        private ILanguageService _language;
        private INavigationService _navigation;
        #endregion

        #region Properties
        public static IServiceProvider Services { get; set; }
        #endregion

        #region Constructor
        public App()
        {
            InitializeComponent();
            XF.Material.Forms.Material.Init(this);

            InitApp();
        }
        #endregion

        #region Methods
        private void InitApp()
        {
            _theme = (IThemeService)Services.GetService(typeof(IThemeService));
            _language = (ILanguageService)Services.GetService(typeof(ILanguageService));
            _navigation = (INavigationService)Services.GetService(typeof(INavigationService));
        }

        protected override void OnStart()
        {
            _theme.DetermineAndLoadTheme();
            _language.DetermineAndSetLanguage();
            _navigation.DetermineAndSetMainPage();
        }
        #endregion
    }
}