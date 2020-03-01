using Newtonsoft.Json;
using System;

namespace TomskGO.Models.VK
{
    public class VKUserModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        
        [JsonProperty("photo_max")]
        public string AvatarUrl { get; set; }
    }

    public class VKUsersInformationRequestModel
    {
        public string user_ids { get; set; }

        public string fields { get; set; }

        public string access_token => Environment.GetEnvironmentVariable("VkAccessToken");

        public string v => Environment.GetEnvironmentVariable("VkApiVersion");
    }
}