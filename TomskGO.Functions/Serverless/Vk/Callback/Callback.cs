using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TomskGO.Functions.API.Public.Local;
using TomskGO.Models.API;
using TomskGO.Models.VK;
using static TomskGO.Models.VK.VKFeedModel;

namespace TomskGO.Functions.Serverless.Vk.Callback
{
    public static class Callback
    {
        private static ILogger _log;

        private static OkObjectResult OkResult => new OkObjectResult("ok");
        private static OkObjectResult ConfirmResult => new OkObjectResult("9a39674c");
        private static NotFoundResult NotFoundResult => new NotFoundResult();
        private static string BaseAddress => Environment.GetEnvironmentVariable("ApiBaseAddress");

        [FunctionName("VkCallback")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,
                         "post",
                         Route = "vk_callback")] HttpRequest req,
            ILogger log)
        {
            _log = log;

            _log.LogInformation("Received callback from vk.");
            var streamReader = new StreamReader(req.Body);
            var requestBody = await streamReader.ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<VkCallbackModel>(requestBody);

            if (data is null)
            {
                _log.LogError("Request object couldn't be deserialized.");
                return NotFoundResult;
            }
            else _log.LogInformation("Successfully deserialized request object.");

            var result = data.Type switch
            {
                //Can be extended in future releases;
                "confirmation" => ConfirmResult,
                "wall_post_new" => await CreateNewsItemAsync(data.RequestObject as Post),
                _ => NotFoundResult
            };

            return result;
        }

        private static async Task<IActionResult> CreateNewsItemAsync(Post post)
        {
            _log.LogInformation("Creating local news item.");
            if (post is null && !post.IsRight())
            {
                _log.LogError("Item was null or didn't have required hashtags.");
                return NotFoundResult;
            }

            var newsItem = post.ConvertDataToUniversal();

            if (newsItem is null)
            {
                _log.LogError("Couldn't convert vk post to universal.");
                return NotFoundResult;
            }
            else _log.LogInformation("Successfully converted vk post to universal.");

            var news = RestService.For<INews>(BaseAddress);
            var result = await news.AddNewsItem(newsItem);

            if (result is null)
            {
                _log.LogError("Couldn't add news item.");
                return NotFoundResult;
            }
            else return OkResult;
        }
    }

    public static class NewsExtensions
    {
        private static string[] ServiceHashTags => new[]
        {
            "#tomskgoAPP"
        };

        public static List<NewsTag> CreateTags(this Post post)
        {
            var localList = new List<NewsTag>();
            var regex = new Regex(@"#\w+");
            var tags = regex.Matches(post.Text);
            foreach (Match m in tags)
            {
                if (!ServiceHashTags.Contains(m.Value))
                    localList.Add(new NewsTag { Name = m.Value.Replace("_", " ") });
            }
            localList = localList.Distinct().ToList();
            return localList;
        }

        public static bool IsRight(this Post item) =>
            ServiceHashTags.All(x => item.Text.Contains(x));

        public static Post[] Filter(this Post[] dataList) =>
            dataList.Where(x => x.IsRight()).ToArray();

        public static NewsModel ConvertDataToUniversal(this Post item)
        {
            return new NewsModel
            {
                ShortDescription = item.Text.Length >= 165 ? item.Text.Substring(0, 130) + "..." : item.Text,
                SourceLabel = "vk",
                FullText = item.Text,
                Date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(item.Date).ToLocalTime(),
                PreviewSource = item.Attachments?.FirstOrDefault(x => x.Photo != null)?.Photo.Sizes.FirstOrDefault(x => x.Type == "p").Url,
                Tags = item.CreateTags(),
                Attachments = new NewsAttachment
                {
                    Audios = item.Attachments?.Where(x => x.Audio != null)?.Select(x => new NewsAttachment.Audio()
                    {
                        Artists = x.Audio.MainArtists.Select(a => a.Name).ToList()
                    }).ToList(),
                    Links = item.Attachments?.Where(x => x.Link != null)?.Select(x => new NewsAttachment.Link()
                    {
                        Title = x.Link.Title,
                        Url = x.Link.Url
                    }).ToList(),
                    Photos = item.Attachments?.Where(x => x.Photo != null)?.Select(x => new NewsAttachment.Photo()
                    {
                        ImageSource = x.Photo.Sizes.FirstOrDefault(s => s.Type == "q").Url
                    }).ToList(),
                    AudiosVisible = item.Attachments?.Where(x => x.Audio != null).Count() > 0,
                    PhotosVisible = item.Attachments?.Where(x => x.Photo != null).Count() > 0,
                    LinksVisible = item.Attachments?.Where(x => x.Link != null).Count() > 0
                },
                AttachmentsVisible = item.Attachments?.Any(x => x.Photo != null || x.Link != null || x.Audio != null)
            };
        }
    }
}