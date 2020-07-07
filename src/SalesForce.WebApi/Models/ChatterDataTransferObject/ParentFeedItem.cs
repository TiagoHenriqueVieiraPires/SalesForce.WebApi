using System;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Models.ChatterDataTransferObject
{
    public class ParentFeedItem
    {
        [JsonProperty(PropertyName = "id")]
        public String Id { get; set; }

        [JsonProperty(PropertyName = "type")]
        public String Type { get; set; }
    }
    
}