using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Refit;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TomskGO.Functions.API.Public.VK;
using TomskGO.Functions.Serverless.Vk.Callback;

namespace TomskGO.Functions.Serverless.Vk
{
    public static class FetchPosts
    {
        private static string VkBaseAddress => Environment.GetEnvironmentVariable("VkBaseAddress");

        //[FunctionName("VkFetchPosts")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
                                     [CosmosDB(
                                     databaseName: "News",
                                     collectionName: "NewsFeed",
                                     ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<object> newsContext,
                                     ILogger log)
        {
            var client = new HttpClient(new HttpClientHandler
            {
                UseProxy = false
            })
            {
                BaseAddress = new Uri(VkBaseAddress)
            };
            var vk = RestService.For<IVk>(client);
            await vk.GetPostsAsync(new Models.VK.VKRequestModel
            {
                owner_id = -66471096
            })
                .ContinueWith(async t =>
                {
                    var data = t.Result;
                    var posts = data.response?.Items;
                    var filteredItems = posts.Filter();
                    foreach (var item in filteredItems)
                    {
                        var newsItem = item.ConvertDataToUniversal();
                        await newsContext.AddAsync(new
                        {
                            newsItem.Date,
                            newsItem.ShortDescription,
                            newsItem.FullText,
                            newsItem.SourceLabel,
                            newsItem.PreviewSource,
                            newsItem.Tags,
                            newsItem.Attachments
                        })
                        .ContinueWith(t =>
                        {
                            if (t.IsFaulted || t.IsCanceled)
                            {
                                var error = "Couldn't add news item. Perhaps, you entered a duplicate id. Maybe data has incorrect structure.";
                                log.LogError(error);
                            }

                            if (t.IsCompletedSuccessfully)
                            {
                                log.LogInformation("Added news item: {data}", data);
                            }
                        });
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}