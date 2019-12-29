using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using TomskGO.Core.Services.News;
using TomskGO.Services.News;
using TomskGO.ViewModels;
using TomskGO.Views;

namespace TomskGO.Core
{
    public static class Startup
    {
        public static App Init(Action<HostBuilderContext, IServiceCollection> nativeConfigureServices)
        {
            var host = Host
                .CreateDefaultBuilder()
                .ConfigureServices((c, x) =>
                {
                    nativeConfigureServices(c, x);
                    ConfigureServices(c, x);
                })
                .ConfigureLogging(l => l.AddConsole(o =>
                {
                    o.DisableColors = true;
                }))
                .Build();

            App.Services = host.Services;

            return App.Services.GetService<App>();
        }

        private static void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
        {
            #region Services
            services.AddSingleton<INewsService, NewsService>();
            #endregion

            #region ViewModels
            services.AddSingleton<NewsFeedViewModel>();
            services.AddSingleton<PostViewModel>();
            #endregion

            #region Views
            services.AddSingleton<NewsFeed>();
            services.AddSingleton<Post>();
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
}