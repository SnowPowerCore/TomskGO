using System.Collections.Generic;
using System.Threading.Tasks;
using TomskGO.Models.API;
using TomskGO.Services.Api.News;
using TomskGO.Services.News;

namespace TomskGO.Core.Services.News
{
    public class NewsService : INewsService
    {
        private INewsApi _news;

        public NewsService(INewsApi news)
        {
            _news = news;
        }

        public Task<List<NewsModel>> GetAllNewsAsync() =>
            _news.GetAllNewsAsync<List<NewsModel>>();

        public Task<NewsModel> GetNewsItemByIdAsync(int id) =>
            _news.GetNewsItemByIdAsync<NewsModel>(id);

        public Task<bool> PostNewsItemAsync(NewsModel newItem) =>
            _news.PostNewsItemAsync<bool>(newItem);

        public Task<bool> UpdateNewsItemAsync(int id, NewsModel updated) =>
            _news.UpdateNewsItemAsync<bool>(id, updated);

        public Task<bool> DeleteNewsItemAsync(int id) =>
            _news.DeleteNewsItemAsync<bool>(id);
    }
}