using System.Collections.Generic;
using System.Threading.Tasks;
using TomskGO.Models.API;

namespace TomskGO.Core.Services.Tomsk.News
{
    public interface INewsService
    {
        NewsModel SelectedPost { get; set; }

        string SelectedTagName { get; set; }

        Task<List<NewsModel>> GetAllNewsAsync();

        Task<NewsModel> GetNewsItemByIdAsync(int id);

        Task<bool> PostNewsItemAsync(NewsModel newItem);

        Task<bool> UpdateNewsItemAsync(int id, NewsModel updated);

        Task<bool> DeleteNewsItemAsync(int id);
    }
}