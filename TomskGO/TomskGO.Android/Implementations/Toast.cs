using Android.Widget;
using TomskGO.Core.Interfaces;
using Xamarin.Forms;

namespace TomskGO.Droid.Implementations
{
    public class Toast : IToast
    {
        public void ShowToast(string message)
        {
            Device.InvokeOnMainThreadAsync(() =>
                Android.Widget.Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show());
        }
    }
}