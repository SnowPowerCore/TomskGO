using Refit;
using System.Threading.Tasks;
using TomskGO.Models.API;

namespace TomskGO.Services.Api.News
{
    public interface INewsApi
    {
        [Get("/api/news")]
        Task<TResult> GetAllNewsAsync<TResult>();

        [Get("/api/news/{id}")]
        Task<TResult> GetNewsItemByIdAsync<TResult>(int id);

        [Post("/api/news")]
        Task<TResult> PostNewsItemAsync<TResult>([Body] NewsModel news);

        [Put("/api/news/{id}")]
        Task<TResult> UpdateNewsItemAsync<TResult>(int id, [Body] NewsModel news);

        [Delete("/api/news/{id}")]
        Task<TResult> DeleteNewsItemAsync<TResult>(int id);
    }
}