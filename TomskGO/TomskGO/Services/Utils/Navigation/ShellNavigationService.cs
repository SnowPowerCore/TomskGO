using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TomskGO.Core.Services.Utils.Navigation
{
    public class ShellNavigationService : INavigationService
    {
        public Task SwitchMainPageAsync<TPage>(TPage page)
        {
            if (!(page is Shell)) return Task.CompletedTask;
            Shell.Current.FlyoutIsPresented = false;
            return CloseModalAsync()
                .ContinueWith(t =>
                    Device.InvokeOnMainThreadAsync(() => Application.Current.MainPage = page as Shell),
                        TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public void DetermineAndSetMainPage(string mainRouteName) =>
            Application.Current.MainPage = (Shell)Routing.GetOrCreateContent(mainRouteName);

        public bool CheckCurrentPageType<TType>() =>
            Shell.Current.Navigation.NavigationStack.Count > 1 &&
            Shell.Current.Navigation.NavigationStack.LastOrDefault().GetType().Equals(typeof(TType));

        public Task OpenModalAsync(Page modal, bool animated = true) =>
            Shell.Current.Navigation.PushModalAsync(modal, animated);

        public Task CloseModalAsync(bool animated = true)
        {
            if (Shell.Current.Navigation.ModalStack.Count > 0)
                return Shell.Current.Navigation.PopModalAsync(animated);
            
            return Task.CompletedTask;
        }

        public Task NavigateToPageAsync(string routeWithParams, bool animated = true)
        {
            Shell.Current.FlyoutIsPresented = false;
            return Shell.Current.GoToAsync(routeWithParams, animated);
        }

        public Task NavigateBackAsync(bool animated = true)
        {
            if (Shell.Current.Navigation.NavigationStack.Count > 1)
                return Shell.Current.Navigation.PopAsync(animated);
            return Task.CompletedTask;
        }

        public Task NavigateToRootAsync(bool animated = true)
        {
            Shell.Current.FlyoutIsPresented = false;
            return Shell.Current.Navigation.PopToRootAsync(animated);
        }
    }
}