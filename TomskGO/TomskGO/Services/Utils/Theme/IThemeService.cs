using System.Threading.Tasks;

namespace TomskGO.Core.Services.Utils.Theme
{
    public enum Theme
    {
        Light,
        Dark
    }

    public interface IThemeService
    {
        void DetermineAndLoadTheme();

        Task SwitchThemeAsync();
    }
}