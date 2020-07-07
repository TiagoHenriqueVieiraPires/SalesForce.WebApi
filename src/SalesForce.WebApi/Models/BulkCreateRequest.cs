using System.Collections.Generic;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Models
{
    public class BulkCreateRequest
    {
        [JsonRequired]
        public string Object { get; set; }
        [JsonProperty(PropertyName = "request")]
        public Request Request { get; set; }
    }

    public class Request
    {
        [JsonProperty(PropertyName = "records")]
        public List<object> Records { get; set; }

        public Request()
        {
            Records = new List<object>();
        }
    }
}