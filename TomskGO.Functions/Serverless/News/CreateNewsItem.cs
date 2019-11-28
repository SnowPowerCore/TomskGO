using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using TomskGO.Models.API;

namespace TomskGO.Functions.Serverless.News
{
    public static class CreateNewsItem
    {
        [FunctionName("CreateNewsItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "news")] HttpRequest req,
            [CosmosDB(databaseName: "News",
                      collectionName: "NewsFeed",
                      ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<object> newsContext,
            ILogger log)
        {
            log.LogInformation("Started adding news item.");
            var streamReader = new StreamReader(req.Body);
            var requestBody = await streamReader.ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<NewsModel>(requestBody);

            if (data is null)
            {
                var error = "Couldn't deserialize request object.";
                log.LogError(error);
                return new NotFoundObjectResult(error);
            }

            IActionResult result = new EmptyResult();

            await newsContext.AddAsync(new
            {
                data.Date,
                data.ShortDescription,
                data.FullText,
                data.SourceLabel,
                data.PreviewSource,
                data.Tags,
                data.Attachments
            })
            .ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    var error = "Couldn't add news item. Perhaps, you entered a duplicate id. Maybe data has incorrect structure.";
                    log.LogError(error);
                    result = new BadRequestObjectResult(error);
                }

                if (t.IsCompletedSuccessfully)
                {
                    log.LogInformation("Added news item: {data}", data);
                    result = new OkObjectResult(data);
                }
            });

            return result;
        }
    }
}