using System;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Models.ChatterDataTransferObject
{
    public class FeedItem
    {
        [JsonProperty(PropertyName = "id")]
        public String Id { get; set; }
        
        [JsonProperty(PropertyName = "parent")]
        public ParentFeedItem Parent { get; set; }
    }
}