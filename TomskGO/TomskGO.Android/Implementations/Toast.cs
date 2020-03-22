using AWidget = Android.Widget;
using AApp = Android.App;
using TomskGO.Core.Interfaces;
using Xamarin.Forms;

namespace TomskGO.Android.Implementations
{
    public class Toast : IToast
    {
        public void ShowToast(string message)
        {
            Device.InvokeOnMainThreadAsync(() =>
                AWidget.Toast.MakeText(AApp.Application.Context, message, AWidget.ToastLength.Short).Show());
        }
    }
}