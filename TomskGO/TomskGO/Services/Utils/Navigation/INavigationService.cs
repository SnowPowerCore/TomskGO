using System.Threading.Tasks;
using Xamarin.Forms;

namespace TomskGO.Core.Services.Utils.Navigation
{
    public interface INavigationService
    {
        Task SwitchMainPageAsync<TPage>(TPage page);

        void DetermineAndSetMainPage();

        bool CheckCurrentPageType<TType>();

        Task NavigateToPageAsync(string routeWithParams, bool animated = true);

        Task NavigateBackAsync(bool animated = true);

        Task NavigateToRootAsync(bool animated = true);

        Task OpenModalAsync(Page modal, bool animated = true);

        Task CloseModalAsync(bool animated = true);
    }
}