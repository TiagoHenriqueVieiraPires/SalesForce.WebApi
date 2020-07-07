using System.Collections.Generic;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Models
{
    public class BulkResult
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }
        [JsonProperty(PropertyName = "referenceId")]
        public string ReferenceId { get; set; }
        [JsonProperty(PropertyName = "errors")]
        public List<Error> Errors { get; set; }
    }
}