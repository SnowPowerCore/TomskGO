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

        public async Task<List<NewsModel>> GetAllNewsAsync() =>
            await _news.GetAllNewsAsync<List<NewsModel>>();

        public async Task<NewsModel> GetNewsItemByIdAsync(int id) =>
            await _news.GetNewsItemByIdAsync<NewsModel>(id);

        public async Task<bool> PostNewsItemAsync(NewsModel newItem) =>
            await _news.PostNewsItemAsync<bool>(newItem);

        public async Task<bool> UpdateNewsItemAsync(int id, NewsModel updated) =>
            await _news.UpdateNewsItemAsync<bool>(id, updated);

        public async Task<bool> DeleteNewsItemAsync(int id) =>
            await _news.DeleteNewsItemAsync<bool>(id);
    }
}