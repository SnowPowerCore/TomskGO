using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using TomskGO.Models.API;

namespace TomskGO.Functions.Serverless.News
{
    public static class GetNewsItemById
    {
        [FunctionName("GetNewsItemById")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "news/{id}")] HttpRequest req,
            [CosmosDB(databaseName: "News",
                      collectionName: "NewsFeed",
                      ConnectionStringSetting = "CosmosDBConnection",
                      Id = "{id}")] NewsModel newsItem,
            ILogger log)
        {
            log.LogInformation("Retrieving news item.");

            if (newsItem is null)
            {
                log.LogError("News item is null. Provided id: ", req.Query["id"], ".");
                return new NotFoundResult();
            }

            log.LogInformation("Retrieved news item.");
            return new OkObjectResult(newsItem);
        }
    }
}
