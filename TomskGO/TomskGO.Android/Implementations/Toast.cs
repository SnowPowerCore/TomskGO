using Android.Widget;
using TomskGO.Core.Interfaces;
using Xamarin.Forms;
using Application = Android.App.Application;
using AToast = Android.Widget.Toast;

namespace TomskGO.Android.Implementations
{
    public class Toast : IToast
    {
        public void ShowToast(string message) =>
            Device.InvokeOnMainThreadAsync(() =>
                AToast.MakeText(Application.Context, message, ToastLength.Short).Show());
    }
}