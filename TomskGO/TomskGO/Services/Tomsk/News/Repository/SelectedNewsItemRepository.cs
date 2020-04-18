using TomskGO.Models.API;

namespace TomskGO.Core.Services.Tomsk.News.Repository
{
    public class SelectedNewsItemRepository : INewsRepository
    {
        private NewsModel _news;

        private string _tagName;

        public NewsModel GetNewsItem() => _news;

        public string GetTagName() => _tagName;

        public void SetNewsItem(NewsModel newsItem) =>
            _news = newsItem;

        public void SetTagName(string tagName) =>
            _tagName = tagName;
    }
}