using System;
using TomskGO.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TomskGO
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            XF.Material.Forms.Material.Init(this);
            MainPage = new Views.MainPage();
        }

        protected override void OnStart()
        {
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute("post", typeof(Post));
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
