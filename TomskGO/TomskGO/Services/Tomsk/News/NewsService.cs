using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;
using TomskGO.Core.Services.Tomsk.Api.News;
using TomskGO.Core.Services.Tomsk.RestClient;
using TomskGO.Models.API;

namespace TomskGO.Core.Services.Tomsk.News
{
    public class NewsService : INewsService
    {
        private INewsApi _news;

        public NewsModel SelectedPost { get; set; }

        public string SelectedTagName { get; set; }

        public NewsService(IRestClientProvider client)
        {
            _news = RestService.For<INewsApi>(client.BetrouteClient);
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