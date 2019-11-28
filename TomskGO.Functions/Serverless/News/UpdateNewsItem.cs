using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TomskGO.Models.API;

namespace TomskGO.Functions.Serverless.News
{
    public static class UpdateNewsItem
    {
        [FunctionName("UpdateNewsItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "news/{id}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "CosmosDBConnection")] DocumentClient newsDb,
            ILogger log,
            string id)
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

            var collectionUri = UriFactory.CreateDocumentCollectionUri("News", "NewsFeed");
            var newsItem = newsDb.CreateDocumentQuery(collectionUri)
                .FirstOrDefault(t => t.Id == id);

            if (newsItem is null)
            {
                log.LogError("Couldn't find requested news item.");
                return new NotFoundResult();
            }

            newsItem.SetPropertyValue("Date", data.Date);
            newsItem.SetPropertyValue("ShortDescription", data.ShortDescription);
            newsItem.SetPropertyValue("FullText", data.FullText);
            newsItem.SetPropertyValue("SourceLabel", data.SourceLabel);
            newsItem.SetPropertyValue("PreviewSource", data.PreviewSource);
            newsItem.SetPropertyValue("Attachments", data.Attachments);
            newsItem.SetPropertyValue("Tags", data.Tags);

            IActionResult result = new EmptyResult();

            await newsDb.ReplaceDocumentAsync(newsItem)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        var error = "Couldn't replace news item. Perhaps, data has incorrect structure.";
                        log.LogError(error);
                        result = new BadRequestObjectResult(error);
                    }

                    if (t.IsCompletedSuccessfully)
                    {
                        log.LogInformation("Replaced news item: {data}", data);
                        result = new OkObjectResult(newsItem);
                    }
                });

            return result;
        }
    }
}
