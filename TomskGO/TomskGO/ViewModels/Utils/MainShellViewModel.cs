using TomskGO.Core.Services.Utils.Navigation;

namespace TomskGO.Core.ViewModels.Utils
{
    public class MainShellViewModel : BaseViewModel
    {
        private readonly INavigationService _navigation;

        public MainShellViewModel(INavigationService navigation)
        {
            _navigation = navigation;
        }

        public bool? CheckCanExit() =>
            _navigation.CheckCanExit();
    }
}
