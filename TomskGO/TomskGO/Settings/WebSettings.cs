using Xamarin.Forms;

namespace BSWApp.Settings
{
    /// <summary>
    /// Web URLs, sorted by specific http method or provider
    /// </summary>
    static class WebSettings
    {
        public static string WEB_URL => (string)Application.Current.Properties["WebServer"];
        public static string WEB_URL_DEFAULT => PROTOCOL + "api.vk.com";
        public static string PROTOCOL => "https://";

        public static class GetMethodUrls
        {
        }

        public static class PostMethodUrls
        {
        }

        public static class PutMethodUrls
        {
        }

        public static class DeleteMethodUrls
        {
        }
    }
}