using TomskGO.Core.ViewModels.Utils;

namespace TomskGO.Core.Views.Shells
{
    public partial class MainPage
    {
        public MainShellViewModel MainShellViewModel =>
            (MainShellViewModel)BindingContext;

        public MainPage() =>
            InitializeComponent();

        protected override bool OnBackButtonPressed()
        {
            var result = MainShellViewModel?.CheckCanExit();
            if (result is null) return base.OnBackButtonPressed();
            return true;
        }
    }
}