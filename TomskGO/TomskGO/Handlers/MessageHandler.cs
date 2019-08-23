using System.Threading.Tasks;
using Xamarin.Forms;

namespace TomskGO.Handlers
{
    /// <summary>
    /// Displays dialog screens
    /// </summary>
    internal static class MessageHandler
    {
        /// <summary>
        /// Displays information
        /// </summary>
        /// <param name="infoName">Information title</param>
        /// <param name="infoDesc">Information description</param>
        public static void DisplayInfo(string infoName, string infoDesc)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.DisplayAlert(infoName, infoDesc, "OK");
            });
        }

        /// <summary>
        /// Displays error only
        /// </summary>
        /// <param name="errorDesc">Error description</param>
        public static void DisplayErrorDescOnly(string errorDesc)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.DisplayAlert("Error", errorDesc, "OK");
            });
        }

        /// <summary>
        /// Display confirmation dialog
        /// </summary>
        /// <param name="dialogName">Dialog title</param>
        /// <param name="dialogDesc">Dialog description</param>
        /// <param name="confirmLabel">Confirm button text</param>
        /// <param name="denyLabel">Deny button text</param>
        /// <returns></returns>
        public static Task<bool> DisplayConfirmation(string dialogName, string dialogDesc, string confirmLabel, string denyLabel)
        {
            var tcs = new TaskCompletionSource<bool>();
            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await Application.Current.MainPage.DisplayAlert(dialogName, dialogDesc, confirmLabel, denyLabel);
                tcs.SetResult(result);
            });
            return tcs.Task;
        }
    }
}