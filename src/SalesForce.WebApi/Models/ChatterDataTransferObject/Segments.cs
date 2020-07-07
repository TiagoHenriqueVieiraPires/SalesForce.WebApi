using System;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Models.ChatterDataTransferObject
{
    public class Segments
    {
        [JsonProperty(PropertyName = "text")]
        public String Text { get; set; }

        [JsonProperty(PropertyName = "type")]
        public String Type { get; set; }
    }
}