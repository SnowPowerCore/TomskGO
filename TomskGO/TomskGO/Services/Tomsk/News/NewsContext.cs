using TomskGO.Core.Services.Tomsk.News.Repository;
using TomskGO.Core.Services.Tomsk.News.Service;

namespace TomskGO.Core.Services.Tomsk.News
{
    public class NewsContext
    {
        public INewsRepository NewsRepository { get; private set; }

        public INewsService NewsService { get; private set; }

        public NewsContext(INewsService newsService,
                           INewsRepository newsRepository)
        {
            NewsService = newsService;
            NewsRepository = newsRepository;
        }
    }
}