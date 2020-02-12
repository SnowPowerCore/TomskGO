using System;
using System.Net.Http;
using TomskGO.Core.Services.Utils.Settings;

namespace TomskGO.Core.Services.Tomsk.RestClient
{
    public class RestClientProvider : IRestClientProvider
    {
        private readonly HttpClientHandler _handler;

        public HttpClient BetrouteClient { get; private set; }

        public RestClientProvider(ISettingsService settings)
        {
            _handler = new HttpClientHandler
            {
                UseCookies = true,
                UseProxy = false
            };

            BetrouteClient = new HttpClient(_handler)
            {
                BaseAddress = new Uri(settings.DefaultApiUrl)
            };
        }
    }
}