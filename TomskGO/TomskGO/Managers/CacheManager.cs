using System;
using TomskGO.ViewModels;
using Xamarin.Forms;

namespace TomskGO.Managers
{
    /// <summary>
    /// Application cache manager
    /// </summary>
    static class CacheManager
    {
        #region Private Fields
        private static Lazy<NewsFeedViewModel> _newsFeed = new Lazy<NewsFeedViewModel>(() => GetResource<NewsFeedViewModel>("newsFeed"));
        #endregion

        #region Views

        #endregion

        #region ViewModels
        public static NewsFeedViewModel NewsFeed => _newsFeed.Value;
        #endregion

        public static T GetResource<T>(string name) where T : class
        {
            if (Application.Current.Resources.ContainsKey(name))
            {
                return (T)Application.Current.Resources[name];
            }
            else
            {
                return (T)(Application.Current.Resources[name] = Activator.CreateInstance<T>());
            }
        }
    }
}