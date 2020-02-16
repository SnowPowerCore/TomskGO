using System.Collections.Generic;
using System.Threading.Tasks;
using TomskGO.Core.Services.Utils.Settings;
using TomskGO.Themes;
using Xamarin.Forms;

namespace TomskGO.Core.Services.Utils.Theme
{
    public class ThemeService : IThemeService
    {
        private ISettingsService _settings;

        private Dictionary<Theme, ResourceDictionary> ThemeResources =>
            new Dictionary<Theme, ResourceDictionary>
            {
                { Theme.Light, new LightTheme() },
                { Theme.Dark, new DarkTheme() }
            };

        private ICollection<ResourceDictionary> MergedDictionaries =>
            Application.Current.Resources.MergedDictionaries;

        public ThemeService(ISettingsService settings)
        {
            _settings = settings;
        }

        public void DetermineAndLoadTheme()
        {
            var current = CheckTheme();

            MergedDictionaries.Clear();
            MergedDictionaries.Add(ThemeResources[current]);
        }

        public Task SwitchThemeAsync()
        {
            var current = CheckTheme();

            MergedDictionaries.Clear();

            switch (current)
            {
                case Theme.Dark:
                    MergedDictionaries.Add(ThemeResources[Theme.Light]);
                    return _settings.AddOrUpdateValueAsync("currentTheme", Theme.Light, true);
                case Theme.Light:
                    MergedDictionaries.Add(ThemeResources[Theme.Dark]);
                    return _settings.AddOrUpdateValueAsync("currentTheme", Theme.Dark, true);
            }

            return Task.CompletedTask;
        }

        private Theme CheckTheme()
        {
            if (!_settings.ContainsKey("currentTheme"))
            {
                var current = Theme.Light;
                _settings.AddOrUpdateValueAsync("currentTheme", current, true);
                return current;
            }
            return _settings.GetValueOrDefault<Theme>("currentTheme", deserialize: true);
        }
    }
}