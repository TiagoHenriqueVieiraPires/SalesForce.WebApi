using System.Collections.Generic;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Models.ChatterDataTransferObject
{
    public class MessageInputBody
    {
        [JsonProperty(PropertyName = "messageSegments")]
        public List<Segments> MessageSegments { get; set; }
    }
}