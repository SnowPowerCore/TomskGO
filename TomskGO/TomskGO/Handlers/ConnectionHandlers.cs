using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TomskGO.Handlers
{
    /// <summary>
    /// Abstract web handler with all the useful methods
    /// </summary>
    public abstract class WebHandler
    {
        /// <summary>
        /// Base address of the website
        /// </summary>
        protected abstract Uri BaseAddress { get; }
        /// <summary>
        /// Is current user signed in
        /// </summary>
        public static bool IsSignedIn { get; set; }

        /// <summary>
        /// Container for the web cookies
        /// </summary>
        protected virtual CookieContainer CookieContainer { get; set; } = new CookieContainer();
        /// <summary>
        /// Http client handler
        /// </summary>
        protected virtual HttpClientHandler Handler => new HttpClientHandler()
        {
            UseCookies = true,
            CookieContainer = CookieContainer,
            AutomaticDecompression = DecompressionMethods.GZip
        };
        /// <summary>
        /// Abstract representation of the http client
        /// </summary>
        protected abstract HttpClient _client { get; }

        /// <summary>
        /// Sends a request to the desired route
        /// </summary>
        /// <param name="url">Route url</param>
        /// <param name="method">Http method type</param>
        /// <param name="body">Additional data</param>
        /// <returns>Response string</returns>
        public async Task<string> SendAsync(string url, HttpMethod method, string body = null)
        {
            try
            {
                using (var message = new HttpRequestMessage(method, url))
                {
                    if (body != null)
                        message.Content = new StringContent(body, Encoding.UTF8, "application/json");
                    using (var result = await _client?.SendAsync(message, HttpCompletionOption.ResponseHeadersRead))
                    {
                        if (!result.IsSuccessStatusCode)
                        {
                            return null;
                        }
                        var bytes = await result.Content.ReadAsByteArrayAsync();
                        return Encoding.UTF8.GetString(bytes).TrimEnd('\0');
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Registers user
        /// </summary>
        /// <param name="url">Route url</param>
        /// <param name="method">Http method type</param>
        /// <param name="body">User data</param>
        /// <returns>Response string</returns>
        public async Task<string> RegisterAsync(string url, HttpMethod method, string body)
        {
            try
            {
                using (var message = new HttpRequestMessage(method, url))
                {
                    message.Content = new StringContent(body);
                    using (var result = await _client?.SendAsync(message, HttpCompletionOption.ResponseHeadersRead))
                    {
                        if (!result.IsSuccessStatusCode)
                        {
                            MessageHandler.DisplayErrorDescOnly(result.ReasonPhrase);
                            return null;
                        }
                        var bytes = await result.Content.ReadAsByteArrayAsync();
                        return Encoding.UTF8.GetString(bytes).TrimEnd('\0');
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Authentificates user
        /// </summary>
        /// <param name="url">Route url</param>
        /// <param name="body">User data</param>
        /// <returns>Response string</returns>
        public async Task<string> LoginAsync(string url, string body)
        {
            try
            {
                using (var message = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    message.Content = new StringContent(body);
                    using (var result = await _client?.SendAsync(message, HttpCompletionOption.ResponseHeadersRead))
                    {
                        if (!result.IsSuccessStatusCode)
                        {
                            MessageHandler.DisplayErrorDescOnly(result.ReasonPhrase);
                            return null;
                        }
                        var bytes = await result.Content.ReadAsByteArrayAsync();
                        return Encoding.UTF8.GetString(bytes).TrimEnd('\0');
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Deauthentificates user
        /// </summary>
        /// <returns>Task</returns>
        public async Task LogoutAsync()
        {
            _client.CancelPendingRequests();
            var cookiesList = CookieContainer.GetCookies(BaseAddress).Cast<Cookie>().ToList();
            foreach (Cookie cookie in cookiesList)
            {
                Application.Current.Properties.Remove(cookie.Name);
                cookie.Expired = true;
            }
            Application.Current.Properties.Remove("IsSignedIn");
            Application.Current.Properties.Remove("CookiesSet");
            await Application.Current.SavePropertiesAsync();
            IsSignedIn = false;
        }

        /// <summary>
        /// Resets the cookies in a cookie container
        /// </summary>
        internal void ResetClient()
        {
            CookieContainer.Add(BaseAddress, new Cookie("PHPSESSID", Application.Current.Properties["PHPSESSID"] as string));
            CookieContainer.Add(BaseAddress, new Cookie("TOMSK", Application.Current.Properties["TOMSK"] as string));
            CookieContainer.Add(BaseAddress, new Cookie("session", Application.Current.Properties["session"] as string));
            if (Application.Current.Properties.ContainsKey("_ga")) CookieContainer.Add(BaseAddress, new Cookie("_TOMSK1", Application.Current.Properties["_TOMSK1"] as string));
            if (Application.Current.Properties.ContainsKey("_TOMSK1")) CookieContainer.Add(BaseAddress, new Cookie("_TOMSK1", Application.Current.Properties["_TOMSK1"] as string));
            if (Application.Current.Properties.ContainsKey("ASP_NET_SessionId")) CookieContainer.Add(BaseAddress, new Cookie("ASP_NET_SessionId", Application.Current.Properties["ASP_NET_SessionId"] as string));
        }

        /// <summary>
        /// Retrieves cookies from the cookie container
        /// </summary>
        /// <returns>Collection of cookies</returns>
        public CookieCollection GetCookies()
        {
            return CookieContainer.GetCookies(BaseAddress);
        }

        /// <summary>
        /// Sets cookies, retrieved from the desired url
        /// </summary>
        /// <param name="url">Route url</param>
        internal async void SetCookies(string url)
        {
            var cookiesList = CookieContainer.GetCookies(new Uri(BaseAddress + url)).Cast<Cookie>().ToList();
            foreach (Cookie cookie in cookiesList)
            {
                Application.Current.Properties[cookie.Name] = cookie.Value;
                CookieContainer.Add(BaseAddress, new Cookie(cookie.Name, cookie.Value));
            }
            Application.Current.Properties["IsSignedIn"] = true;
            Application.Current.Properties["CookiesSet"] = true;
            Application.Current.Properties["IsSignedIn"] = true;
            await Application.Current.SavePropertiesAsync();
            IsSignedIn = true;
        }
    }
}