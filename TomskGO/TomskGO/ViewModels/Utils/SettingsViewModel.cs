using AsyncAwaitBestPractices.MVVM;
using TomskGO.Core.Services.Utils.Theme;

namespace TomskGO.Core.ViewModels.Utils
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IThemeService _theme;

        public IAsyncCommand SwitchThemeCommand =>
            new AsyncCommand(_theme.SwitchThemeAsync);

        public SettingsViewModel(IThemeService theme)
        {
            _theme = theme;
        }
    }
}