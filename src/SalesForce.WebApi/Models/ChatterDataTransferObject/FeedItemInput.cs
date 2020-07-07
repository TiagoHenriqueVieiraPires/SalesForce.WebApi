using System;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Models.ChatterDataTransferObject
{
    public class FeedItemInput
    {
        [JsonProperty(PropertyName = "feedElementType")]
        public String FeedElementType { get; set; }

        [JsonProperty(PropertyName = "subjectId")]
        public String SubjectId { get; set; }

        [JsonProperty(PropertyName = "visibility")]
        public String Visibility { get; set; }

        [JsonProperty(PropertyName = "body")]
        public MessageInputBody Body { get; set; }
    }
}