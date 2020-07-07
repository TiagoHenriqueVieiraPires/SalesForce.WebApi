using System.Collections.Generic;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Models
{
    public class BulkUpdateRequest
    {
        [JsonProperty(PropertyName = "allOrNone")]
        public bool AllOrNone { get; set; }
        [JsonProperty(PropertyName = "records")]
        public List<object> Records { get; set; }

        public BulkUpdateRequest()
        {
            Records = new List<object>();
        }
    }
}