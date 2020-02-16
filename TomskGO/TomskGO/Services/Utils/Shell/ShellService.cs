using System.Linq;
using System.Threading.Tasks;
using TomskGO.Core.Services.Utils.LocalAuth;
using TomskGO.Core.Views.Shells;
using Xamarin.Forms;

namespace TomskGO.Core.Services.Utils.Shell
{
    public class ShellService : IShellService
    {
        private readonly ILocalAuthService _localAuth;

        public Xamarin.Forms.Shell Current => Xamarin.Forms.Shell.Current;

        public ShellService(ILocalAuthService localAuth)
        {
            _localAuth = localAuth;
        }

        public Task<bool> ChangeShellAsync<TShell>(TShell shell) where TShell : Xamarin.Forms.Shell
        {
            Current.FlyoutIsPresented = false;
            CloseModalAsync()
                .ContinueWith(t =>
                    Device.InvokeOnMainThreadAsync(() => Application.Current.MainPage = shell),
                        TaskContinuationOptions.OnlyOnRanToCompletion);
            return Task.FromResult(true);
        }

        public void DetermineAndSetCurrent() =>
            Application.Current.MainPage = new MainPage();

        public bool CheckCurrentPageType<T>() where T : Page =>
            Current.Navigation.NavigationStack.Count > 1 &&
            Current.Navigation.NavigationStack.LastOrDefault().GetType().Equals(typeof(T));

        public Task OpenModalAsync(Page modal, bool animated = true) =>
            Current.Navigation.PushModalAsync(modal, animated);

        public Task CloseModalAsync(bool animated = true)
        {
            if (Current.Navigation.ModalStack.Count > 0)
                return Current.Navigation.PopModalAsync(animated);
            return Task.CompletedTask;
        }

        public Task NavigateToPageAsync(string routeWithParams, bool animated = true)
        {
            Current.FlyoutIsPresented = false;
            return Current.GoToAsync(routeWithParams, animated);
        }

        public Task NavigateToPageAsync(Page page, bool animated = true)
        {
            Current.FlyoutIsPresented = false;
            return Current.Navigation.PushAsync(page, animated);
        }

        public Task NavigateBackAsync(bool animated = true)
        {
            if (Current.Navigation.NavigationStack.Count > 1)
                return Current.Navigation.PopAsync(animated);
            return Task.CompletedTask;
        }

        public Task NavigateToRootAsync(bool animated = true)
        {
            Current.FlyoutIsPresented = false;
            return Current.Navigation.PopToRootAsync(animated);
        }

        public bool CheckCanExit() =>
            Current.Navigation.NavigationStack.Count == 1;
    }
}