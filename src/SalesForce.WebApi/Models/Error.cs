using System.Collections.Generic;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Models
{
    public class Error
    {
        [JsonProperty(PropertyName = "statusCode")]
        public string StatusCode { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "fields")]
        public List<string> Fields { get; set; }
    }
}