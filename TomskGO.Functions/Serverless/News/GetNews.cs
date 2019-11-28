using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using TomskGO.Models.API;

namespace TomskGO.Functions.Serverless.News
{
    public static class GetNews
    {
        private static IActionResult NotFoundResult => new NotFoundResult();

        [FunctionName("GetNews")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "news")] HttpRequest req,
            [CosmosDB(databaseName: "News",
                      collectionName: "NewsFeed",
                      ConnectionStringSetting = "CosmosDBConnection",
                      SqlQuery = "SELECT * FROM c order by c.Date desc")]
                      IEnumerable<NewsModel> news,
            ILogger log)
        {
            log.LogInformation("Retrieved news.");
            return new OkObjectResult(news) ?? NotFoundResult;
        }
    }
}