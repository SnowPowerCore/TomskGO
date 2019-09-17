using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TomskGO.Models;
using static TomskGO.Models.VKFeedModel;

namespace TomskGO.Providers
{
    class VKProvider : AbstractFeedProvider
    {
        List<string> ServiceHashTags => new List<string>
        {
            "#tomskgoAPP"
        };

        protected override FeedProviderData ProviderData => new FeedProviderData
        {
            Name = "vk",
            BaseUrl = "https://api.vk.com",
            ProvisionUrl = "/method/wall.get?access_token={0}&owner_id=-{1}&filter={2}&extended={3}&v={4}"
        };

        protected VKRequestModel RequestData { get; }

        protected override HttpClient _client => new HttpClient(Handler) { BaseAddress = new Uri(ProviderData.BaseUrl) };

        public VKProvider(VKRequestModel requestData)
        {
            RequestData = requestData;
        }

        public List<FeedModel> ConvertDataToUniversal(Post[] dataList)
        {
            var localList = new List<FeedModel>();
            for (int i = 0; i < dataList.Length; i++)
            {
                localList.Add(new FeedModel
                {
                    ShortDescription = dataList[i].Text.Length >= 165 ? dataList[i].Text.Substring(0, 130) + "..." : dataList[i].Text,
                    SourceLabel = ProviderData.Name,
                    FullText = dataList[i].Text,
                    Date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(dataList[i].Date).ToLocalTime(),
                    PreviewSource = dataList[i].Attachments?.FirstOrDefault(x => x.Photo != null)?.Photo.Sizes.FirstOrDefault(x => x.Type == "p").Url,
                    Tags = CreateTags(dataList[i]),
                    Attachments = new FeedModel.Attachment
                    {
                        Audios = dataList[i].Attachments?.Where(x => x.Audio != null)?.Select(x => new FeedModel.Attachment.Audio()
                        {
                            Artists = x.Audio.MainArtists.Select(a => a.Name).ToList()
                        }).ToList(),
                        Links = dataList[i].Attachments?.Where(x => x.Link != null)?.Select(x => new FeedModel.Attachment.Link()
                        {
                            Title = x.Link.Title,
                            Url = x.Link.Url
                        }).ToList(),
                        Photos = dataList[i].Attachments?.Where(x => x.Photo != null)?.Select(x => new FeedModel.Attachment.Photo()
                        {
                            ImageSource = x.Photo.Sizes.FirstOrDefault(s => s.Type == "q").Url
                        }).ToList(),
                        AudiosVisible = dataList[i].Attachments?.Where(x => x.Audio != null).Count() > 0,
                        PhotosVisible = dataList[i].Attachments?.Where(x => x.Photo != null).Count() > 0,
                        LinksVisible = dataList[i].Attachments?.Where(x => x.Link != null).Count() > 0
                    },
                    AttachmentsVisible = dataList[i].Attachments.Any(x => x.Photo != null || x.Link != null || x.Audio != null)
                });
            }
            return localList;
        }

        public List<string> CreateTags(Post post)
        {
            var localList = new List<string>();
            var regex = new Regex(@"#\w+");
            var tags = regex.Matches(post.Text);
            foreach(Match m in tags)
            {
                if(!ServiceHashTags.Contains(m.Value))
                localList.Add(m.Value.Replace("_", " "));
            }
            localList = localList.Distinct().ToList();
            return localList;
        }

        public Post[] Filter(Post[] dataList)
        {
            string filterStr = "";
            foreach (var str in ServiceHashTags) filterStr = filterStr + str + " ";
            return dataList.Where(x => x.Text.Contains(filterStr)).ToArray();
        }

        public override async Task<ObservableRangeCollection<FeedModel>> ProvideData()
        {
            var formattedUrl = string.Format(ProviderData.ProvisionUrl, RequestData.access_token, RequestData.owner_id, RequestData.filter, RequestData.extended, RequestData.version);
            var response = await SendAsync(formattedUrl, HttpMethod.Get);
            var responseObj = JObject.Parse(response);
            var listVKModels = responseObj.SelectToken("response").ToObject<VKFeedModel>().Items;
            var filteredItems = Filter(listVKModels);
            var listOfUniversalModels = ConvertDataToUniversal(filteredItems);
            return new ObservableRangeCollection<FeedModel>(listOfUniversalModels);
        }
    }
}