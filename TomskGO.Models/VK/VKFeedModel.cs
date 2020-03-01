using Newtonsoft.Json;
using System;

namespace TomskGO.Models.VK
{
    public class VKFeedModel
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("items")]
        public Post[] Items { get; set; }
        [JsonProperty("profiles")]
        public Profile[] Profiles { get; set; }
        [JsonProperty("groups")]
        public Group[] Groups { get; set; }

        public class Post
        {
            [JsonProperty("id")]
            public long ID { get; set; }

            [JsonProperty("date")]
            public double Date { get; set; }
            [JsonProperty("text")]
            public string Text { get; set; }
            [JsonProperty("attachments")]
            public Attachment[] Attachments { get; set; }
        }

        public class Profile
        {
        }

        public class Group
        {
        }

        public class Attachment
        {
            [JsonProperty("doc")]
            public Doc Doc { get; set; }
            [JsonProperty("photo")]
            public Photo Photo { get; set; }
            [JsonProperty("audio")]
            public Audio Audio { get; set; }
            [JsonProperty("link")]
            public Link Link { get; set; }
        }

        public class Photo
        {
            [JsonProperty("sizes")]
            public Size[] Sizes { get; set; }
            [JsonProperty("text")]
            public string Text { get; set; }
            [JsonProperty("date")]
            public double Date { get; set; }

            public class Size
            {
                [JsonProperty("type")]
                public string Type { get; set; }
                [JsonProperty("url")]
                public string Url { get; set; }
                [JsonProperty("width")]
                public int Width { get; set; }
                [JsonProperty("height")]
                public int Height { get; set; }
            }
        }

        public class Link
        {
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("description")]
            public string Description { get; set; }
            [JsonProperty("caption")]
            public string Caption { get; set; }
        }

        public class Video
        {
            [JsonProperty("src")]
            public string Source { get; set; }
            [JsonProperty("file_size")]
            public double FileSize { get; set; }
            [JsonProperty("width")]
            public int Width { get; set; }
            [JsonProperty("height")]
            public int Height { get; set; }
        }

        public class Audio
        {
            [JsonProperty("artist")]
            public string Artist { get; set; }
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("duration")]
            public int Duration { get; set; }
            [JsonProperty("date")]
            public double Date { get; set; }
            [JsonProperty("is_hq")]
            public bool IsHQ { get; set; }
            [JsonProperty("main_artists")]
            public MainArtist[] MainArtists { get; set; }

            public class MainArtist
            {
                [JsonProperty("name")]
                public string Name { get; set; }
            }
        }

        public class Doc
        {
            [JsonProperty("size")]
            public int Size { get; set; }
            [JsonProperty("ext")]
            public string Ext { get; set; }
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("date")]
            public double Date { get; set; }
            [JsonProperty("type")]
            public int Type { get; set; }
            [JsonProperty("preview")]
            public PreviewData Preview { get; set; }

            public class PreviewData
            {
                [JsonProperty("photo")]
                public Photo Photo { get; set; }
                [JsonProperty("video")]
                public Video Video { get; set; }
            }
        }
    }

    public enum PostType
    {
        owner,
        others,
        all
    }

    public class VKNewsFeedRequestModel
    {
        public int owner_id { get; set; }
        public PostType filter { get; set; } = PostType.all;
        public bool extended { get; set; } = false;
        public int count { get; set; } = 100;
        public string access_token => Environment.GetEnvironmentVariable("VkAccessToken");
        public string v => Environment.GetEnvironmentVariable("VkApiVersion");
    }
}