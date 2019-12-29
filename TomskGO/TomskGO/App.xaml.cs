using System;
using TomskGO.Managers;
using TomskGO.Views;
using Xamarin.Forms;

namespace TomskGO
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; set; }

        public App()
        {
            InitializeComponent();
            XF.Material.Forms.Material.Init(this);
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            RegisterRoutes();
            CacheManager.LoadThemeCommand?.Execute(null);
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute("post", typeof(Post));
        }
    }
}