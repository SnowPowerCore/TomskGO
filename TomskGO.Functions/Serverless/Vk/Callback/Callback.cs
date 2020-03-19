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
using TomskGO.Functions.API.Public.VK;
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

            if (newsItem is default(NewsModel))
            {
                _log.LogError("Couldn't convert vk post to universal.");
                return NotFoundResult;
            }
            else _log.LogInformation("Successfully converted vk post to universal.");

            var news = RestService.For<INews>(BaseAddress);

            IActionResult result = new EmptyResult();

            await news.AddNewsItem(newsItem)
                .ContinueWith(t =>
                {
                    var response = t.Result;
                    if (response is null)
                    {
                        _log.LogError("Couldn't add news item.");
                        result = NotFoundResult;
                    }
                    else result = OkResult;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

            return result;
        }
    }

    public static class VKNewsExtensions
    {
        private static string VkApiBaseAddress => Environment.GetEnvironmentVariable("VkApiBaseAddress");

        private static string VkBaseAddress => Environment.GetEnvironmentVariable("VkBaseAddress");

        private static string[] ServiceHashTags => new[]
        {
            "#tomskgoAPP"
        };

        private static string VKFields => "photo_max";

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

        /// <summary>
        /// Retrieves news item members from the source.
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public static List<NewsMember> ExtractLocalMembers(this Post post)
        {
            var links = new List<NewsMember>();
            var regex = new Regex(@"\[.*?\|.*?\]");
            var items = regex.Matches(post.Text);
            var vk = RestService.For<IVK>(VkApiBaseAddress);
            foreach (Match m in items)
            {
                var section = m.Value.Split('|');
                var id = section.FirstOrDefault().Replace("[", "");
                var name = section.LastOrDefault().Replace("]", "");

                var longId = long.Parse(Regex.Match(id, @"\d+").Value);
                var userType = Regex.Match(id, @"[^0-9]+").Value;

                var member = new TaskCompletionSource<NewsMember>();

                switch (userType)
                {
                    case "id":
                        vk.GetUsersInformationAsync(new VKUsersInformationRequestModel
                        {
                            user_ids = longId.ToString(),
                            fields = VKFields
                        })
                            .ContinueWith(t =>
                            {
                                var result = t.Result;

                                member.SetResult(new NewsMember
                                {
                                    Name = name,
                                    ProfileUrl = VkBaseAddress + id,
                                    PictureUrl = result.response?.FirstOrDefault(x => x.Id == longId).AvatarUrl
                                });
                            }, TaskContinuationOptions.OnlyOnRanToCompletion);
                        break;
                    case "club":
                        vk.GetGroupsInformationAsync(new VKGroupsInformationRequestModel
                        {
                            group_ids = longId.ToString(),
                            fields = VKFields
                        })
                            .ContinueWith(t =>
                            {
                                var result = t.Result;

                                member.SetResult(new NewsMember
                                {
                                    Name = name,
                                    ProfileUrl = VkBaseAddress + id,
                                    PictureUrl = result.response?.FirstOrDefault(x => x.Id == longId).AvatarUrl
                                });
                            }, TaskContinuationOptions.OnlyOnRanToCompletion);
                        break;
                    default:
                        break;
                }

                links.Add(member.Task.Result);
            }
            return links;
        }

        /// <summary>
        /// Replaces local vk links (e.g. profile) with their names.
        /// </summary>
        /// <param name="source">Text which contains definitions with [*|*] pattern.</param>
        /// <returns>Filtered string, which will only contain local url name (second item inside definition).</returns>
        public static string ReplaceLocalLinks(this string source)
        {
            var filteredStr = source;
            var regex = new Regex(@"\[.*?\|.*?\]");
            var items = regex.Matches(source);
            foreach (Match m in items)
            {
                var section = m.Value.Split('|');
                var name = section.LastOrDefault().Replace("]", "");
                filteredStr = filteredStr.Replace(m.Value, name);
            }
            return filteredStr;
        }

        /// <summary>
        /// Replaces global links with "⬇️⬇️⬇️" sequence.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ReplaceGlobalLinks(this string source)
        {
            var filteredStr = source;
            var regex = new Regex(@"\b(?:https?://|www\.)\S+\b");
            var items = regex.Matches(source);
            foreach (Match m in items)
                filteredStr = filteredStr.Replace(m.Value, "⬇️⬇️⬇️");
            return filteredStr;
        }

        public static bool IsRight(this Post item) =>
            ServiceHashTags.All(x => item.Text.Contains(x));

        public static Post[] Filter(this Post[] dataList) =>
            dataList.Where(x => x.IsRight()).ToArray();

        public static NewsModel ConvertDataToUniversal(this Post item)
        {
            var filteredNewsText = item.Text
                .ReplaceLocalLinks()
                .ReplaceGlobalLinks();

            if (string.IsNullOrEmpty(filteredNewsText)) return default;

            var members = item.ExtractLocalMembers();

            var newsItem = new NewsModel
            {
                OriginalId = item.ID,
                ShortDescription = filteredNewsText.Length >= 165
                    ? filteredNewsText.Substring(0, 130) + "..."
                    : filteredNewsText,
                SourceLabel = "vk",
                FullText = filteredNewsText,
                Date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                    .AddSeconds(item.Date)
                    .ToLocalTime(),
                PreviewSource = item.Attachments?
                    .FirstOrDefault(x => x.Photo != null)?.Photo.Sizes
                        .FirstOrDefault(x => x.Type == "r").Url,
                Tags = item.CreateTags(),
                Members = members,
                Attachments = new NewsAttachment
                {
                    Audios = item.Attachments?
                        .Where(x => x.Audio != null)?
                        .Select(x => new NewsAttachment.Audio()
                        {
                            Artists = x.Audio.MainArtists.Select(a => a.Name).ToList()
                        })
                        .ToList(),
                    Links = item.Attachments?
                        .Where(x => x.Link != null)?
                        .Select(x => new NewsAttachment.Link()
                        {
                            Title = x.Link.Title,
                            Url = x.Link.Url,
                            FaviconUrl = x.Link.Caption + "/favicon.ico"
                        })
                        .ToList(),
                    Photos = item.Attachments?
                        .Where(x => x.Photo != null)?
                        .Select(x => new NewsAttachment.Photo()
                        {
                            ImageSource = x.Photo.Sizes.LastOrDefault().Url
                        })
                        .ToList(),
                    AudiosVisible = item.Attachments?.Where(x => x.Audio != null).Count() > 0,
                    PhotosVisible = item.Attachments?.Where(x => x.Photo != null).Count() > 0,
                    LinksVisible = item.Attachments?.Where(x => x.Link != null).Count() > 0
                },
                AttachmentsVisible = item.Attachments?
                    .Any(x => x.Photo != null || x.Link != null || x.Audio != null),
                MembersVisible = members.Count > 0
            };

            return newsItem;
        }
    }
}