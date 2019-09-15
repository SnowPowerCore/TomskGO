using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TomskGO.Themes;
using TomskGO.ViewModels;
using Xamarin.Forms;

namespace TomskGO.Managers
{
    /// <summary>
    /// Application cache manager
    /// </summary>
    static class CacheManager
    {
        private enum Themes
        {
            Light,
            Dark
        }

        #region Properties
        public static LightTheme LightTheme => _lightTheme.Value;
        public static DarkTheme DarkTheme => _darkTheme.Value;
        #endregion

        #region Private Fields
        private static Lazy<NewsFeedViewModel> _newsFeed = new Lazy<NewsFeedViewModel>(() => GetResource<NewsFeedViewModel>("newsFeed"));
        private static Lazy<LightTheme> _lightTheme = new Lazy<LightTheme>(() => GetResource<LightTheme>("lightTheme"));
        private static Lazy<DarkTheme> _darkTheme = new Lazy<DarkTheme>(() => GetResource<DarkTheme>("lightTheme"));
        #endregion

        #region Views

        #endregion

        #region ViewModels
        public static NewsFeedViewModel NewsFeed => _newsFeed.Value;
        #endregion

        public static ICommand LoadThemeCommand => SwitchThemeCommand;
        public static ICommand SwitchThemeCommand =>
            new Command(() => TaskManager.Instance.RegisterTask(async () => await SwitchThemeAsync(), true, isUIRelated: true));
        public static async Task SwitchThemeAsync()
        {
            Themes current;
            if (Application.Current.Properties.ContainsKey("currentTheme"))
            {
                if((string)Application.Current.Properties["currentTheme"] == "light")
                {
                    Application.Current.Properties["currentTheme"] = "dark";
                    await Application.Current.SavePropertiesAsync();
                    current = Themes.Dark;
                }
                else
                {
                    Application.Current.Properties["currentTheme"] = "light";
                    await Application.Current.SavePropertiesAsync();
                    current = Themes.Light;
                }
            }
            else
            {
                Application.Current.Properties["currentTheme"] = "dark";
                await Application.Current.SavePropertiesAsync();
                current = Themes.Dark;
            }

            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            mergedDictionaries?.Clear();

            switch (current)
            {
                case Themes.Light:
                    mergedDictionaries.Add(new LightTheme());
                    break;
                case Themes.Dark:
                    mergedDictionaries.Add(new DarkTheme());
                    break;
            }
        }

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