using System.Net.Http;

namespace TomskGO.Core.Services.Tomsk.RestClient
{
    public interface IRestClientProvider
    {
        HttpClient BetrouteClient { get; }
    }
}