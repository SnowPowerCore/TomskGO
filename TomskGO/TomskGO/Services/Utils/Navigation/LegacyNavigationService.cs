using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TomskGO.Core.Services.Utils.Navigation
{
    public class LegacyNavigationService : INavigationService
    {
        public Task SwitchMainPageAsync<TPage>(TPage page)
        {
            if (!(page is Page)) return Task.CompletedTask;
            return CloseModalAsync()
                .ContinueWith(t =>
                    Device.InvokeOnMainThreadAsync(() => Application.Current.MainPage = page as Page),
                        TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public void DetermineAndSetMainPage() =>
            Application.Current.MainPage = new Page();

        public bool CheckCurrentPageType<TType>() =>
            Application.Current.MainPage.Navigation.NavigationStack.Count > 1
                && Application.Current.MainPage.Navigation.NavigationStack
                    .LastOrDefault()
                    .GetType()
                    .Equals(typeof(TType));

        public Task OpenModalAsync(Page modal, bool animated = true) =>
            Application.Current.MainPage.Navigation.PushModalAsync(modal, animated);

        public Task CloseModalAsync(bool animated = true)
        {
            if (Application.Current.MainPage.Navigation.ModalStack.Count > 0)
                return Application.Current.MainPage.Navigation.PopModalAsync(animated);

            return Task.CompletedTask;
        }

        public Task NavigateToPageAsync(string routeWithParams, bool animated = true) =>
            Application.Current.MainPage.Navigation.PushAsync(
                (Page)Routing.GetOrCreateContent(routeWithParams), animated);

        public Task NavigateBackAsync(bool animated = true)
        {
            if (Application.Current.MainPage.Navigation.NavigationStack.Count > 1)
                return Application.Current.MainPage.Navigation.PopAsync(animated);
            return Task.CompletedTask;
        }

        public Task NavigateToRootAsync(bool animated = true) =>
            Application.Current.MainPage.Navigation.PopToRootAsync(animated);
    }
}