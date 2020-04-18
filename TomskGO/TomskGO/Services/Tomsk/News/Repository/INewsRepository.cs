using TomskGO.Models.API;

namespace TomskGO.Core.Services.Tomsk.News.Repository
{
    public interface INewsRepository
    {
        public NewsModel GetNewsItem();

        public string GetTagName();

        public void SetNewsItem(NewsModel newsItem);

        public void SetTagName(string newsItem);
    }
}