using System.Collections.Generic;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Models
{
    public class CustomNotification
    {
        [JsonProperty(PropertyName = "customNotifTypeId")]
        public string CustomNotifTypeId { get; set; }
        [JsonProperty(PropertyName = "recipientIds")]
        public List<string> RecipientIds { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "body")]
        public string Body { get; set; }
        [JsonProperty(PropertyName = "targetId")]
        public string TargetId { get; set; }
    }
}