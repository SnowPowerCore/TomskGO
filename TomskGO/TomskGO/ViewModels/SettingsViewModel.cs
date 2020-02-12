using AsyncAwaitBestPractices.MVVM;
using TomskGO.Core.Services.Utils.Theme;

namespace TomskGO.Core.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly ThemeService _theme;

        public IAsyncCommand SwitchThemeCommand =>
            new AsyncCommand(_theme.SwitchThemeAsync);

        public SettingsViewModel(ThemeService theme)
        {
            _theme = theme;
        }
    }
}