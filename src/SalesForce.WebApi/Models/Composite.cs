using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace SalesForce.WebApi.Models
{
    public class Composite
    {
        [JsonProperty(PropertyName = "method")]
        public string Method { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "referenceId")]
        public string ReferenceId { get; set; }
        [JsonProperty(PropertyName = "body")]
        public dynamic  Body { get; set; }
    }
}