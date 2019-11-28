using Refit;
using System.Threading.Tasks;
using TomskGO.Models.API;

namespace TomskGO.Functions.API.Public.Local
{
    public interface INews
    {
        [Post("/api/news")]
        Task<NewsModel> AddNewsItem([Body] NewsModel newsItem);
    }
}