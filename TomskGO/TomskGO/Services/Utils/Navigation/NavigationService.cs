﻿using System.Threading.Tasks;
using TomskGO.Core.Interfaces;
using TomskGO.Core.Resources;
using TomskGO.Core.Services.Utils.Message;
using TomskGO.Core.Services.Utils.Shell;
using Xamarin.Forms;

namespace TomskGO.Core.Services.Utils.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly IShellService _shell;
        private readonly IMessageService _message;
        private readonly IAppQuit _appQuit;

        private bool _isQuitting = false;

        public NavigationService(IShellService shell,
                                 IMessageService message,
                                 IAppQuit appQuit)
        {
            _shell = shell;
            _message = message;
            _appQuit = appQuit;
        }

        public Task<bool> SwitchMainPageAsync<TShell>(TShell shell) where TShell : Page
        {
            if (shell is Xamarin.Forms.Shell)
                return _shell.ChangeShellAsync(shell as Xamarin.Forms.Shell);
            return Task.FromResult(false);
        }

        public bool DetermineAndSetMainPage()
        {
            _shell.DetermineAndSetCurrent();
            return true;
        }

        public bool CheckCurrentPageType<T>() where T : Page =>
            _shell.CheckCurrentPageType<T>();

        public Task OpenModalAsync(Page modal, bool animated = true) =>
            _shell.OpenModalAsync(modal, animated);

        public Task CloseModalAsync(bool animated = true) =>
            _shell.CloseModalAsync(animated);

        public Task NavigateToPageAsync(string routeWithParams, bool animated = true) =>
            _shell.NavigateToPageAsync(routeWithParams, animated);

        public Task NavigateToPageAsync(Page page, bool animated = true) =>
            _shell.NavigateToPageAsync(page, animated);

        public Task NavigateBackAsync(bool animated = true) =>
            _shell.NavigateBackAsync(animated);

        public Task NavigateToRootAsync(bool animated = true) =>
            _shell.NavigateToRootAsync(animated);

        public bool? CheckCanExit()
        {
            var shellCheck = _shell.CheckCanExit();
            if (shellCheck)
                if (_isQuitting)
                    using (_message.DisplaySnackbarAsync(AppResources.confirmExit, 1500))
                    {
                        _isQuitting = true;
                        Task.Delay(3000)
                            .ContinueWith(t => _isQuitting = false, TaskContinuationOptions.OnlyOnRanToCompletion);
                    }
                else _appQuit.Quit();
            else return null;
            return true;
        }
    }
}