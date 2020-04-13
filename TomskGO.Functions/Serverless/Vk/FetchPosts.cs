using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Refit;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TomskGO.Functions.API.Public.VK;
using TomskGO.Functions.Serverless.Vk.Callback;
using TomskGO.Models.API;

namespace TomskGO.Functions.Serverless.Vk
{
    public static class FetchPosts
    {
        private static string VkApiBaseAddress => Environment.GetEnvironmentVariable("VkApiBaseAddress");

        [FunctionName("VkFetchPosts")]
        public static async Task Run([TimerTrigger("* */20 * * * *")]TimerInfo myTimer,
                                     [CosmosDB(
                                     databaseName: "News",
                                     collectionName: "NewsFeed",
                                     ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<object> newsContext,
                                     ILogger log)
        {
            using var client = new HttpClient(new HttpClientHandler
            {
                UseProxy = false
            })
            {
                BaseAddress = new Uri(VkApiBaseAddress)
            };
            var vk = RestService.For<IVK>(client);
            await vk.GetPostsAsync(new Models.VK.VKNewsFeedRequestModel
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
                        if (newsItem is default(NewsModel)) continue;
                        await newsContext.AddAsync(new
                        {
                            newsItem.OriginalId,
                            newsItem.Date,
                            newsItem.ShortDescription,
                            newsItem.FullText,
                            newsItem.SourceLabel,
                            newsItem.PreviewSource,
                            newsItem.Tags,
                            newsItem.Members,
                            newsItem.MembersVisible,
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