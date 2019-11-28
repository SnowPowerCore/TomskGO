using Newtonsoft.Json;

namespace TomskGO.Models.VK
{
    public class ResponseModel<T>
    {
        [JsonProperty("response")]
        public T response { get; set; }
    }

    public class VkCallbackModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("group_id")]
        public long GroupID { get; set; }

        [JsonProperty("object")]
        public object RequestObject { get; set; }
    }
}