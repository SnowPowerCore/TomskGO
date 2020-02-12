using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TomskGO.Core.Services.Tomsk.News;
using TomskGO.Core.Services.Tomsk.RestClient;
using TomskGO.Core.Services.Utils.Analytics;
using TomskGO.Core.Services.Utils.Language;
using TomskGO.Core.Services.Utils.LocalAuth;
using TomskGO.Core.Services.Utils.Message;
using TomskGO.Core.Services.Utils.Navigation;
using TomskGO.Core.Services.Utils.Settings;
using TomskGO.Core.Services.Utils.Shell;
using TomskGO.Core.Services.Utils.Theme;
using TomskGO.Core.ViewModels.News;
using TomskGO.Views;
using Xamarin.Forms;

namespace TomskGO.Core
{
    public static class Startup
    {
        public static App Init(Action<IServiceCollection> nativeConfigureServices)
        {
            var host = Host
                .CreateDefaultBuilder()
                .ConfigureServices(x =>
                {
                    nativeConfigureServices(x);
                    ConfigureServices(x);
                })
                .RegisterRoutes()
                .Build();

            App.Services = host.Services;

            return App.Services.GetService<App>();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            #region Services
            services.AddSingleton<IAnalyticsService, AnalyticsService>();
            services.AddSingleton<ILanguageService, LanguageService>();
            services.AddSingleton<ILocalAuthService, LocalAuthService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IShellService, ShellService>();
            services.AddSingleton<IThemeService, ThemeService>();

            services.AddSingleton<INewsService, NewsService>();
            services.AddSingleton<IRestClientProvider, RestClientProvider>();
            #endregion

            #region ViewModels
            services.AddSingleton<NewsFeedViewModel>();
            services.AddSingleton<PostViewModel>();
            #endregion

            #region Application
            services.AddSingleton<App>();
            #endregion
        }

        //public static void ExtractSaveResource(string filename, string location)
        //{
        //    var a = Assembly.GetExecutingAssembly();
        //    using (var resFilestream = a.GetManifestResourceStream(filename))
        //    {
        //        if (resFilestream != null)
        //        {
        //            var full = Path.Combine(location, filename);

        //            using (var stream = File.Create(full))
        //            {
        //                resFilestream.CopyTo(stream);
        //            }
        //        }
        //    }
        //}
    }

    static class StartupExtensions
    {
        public static IHostBuilder RegisterRoutes(this IHostBuilder host)
        {
            Routing.RegisterRoute("post", typeof(Post));

            return host;
        }
    }
}