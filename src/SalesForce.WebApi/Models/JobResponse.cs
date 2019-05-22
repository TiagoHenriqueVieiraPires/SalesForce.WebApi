using System;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Models
{
    public class JobResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "operation")]
        public string Operation { get; set; }

        [JsonProperty(PropertyName = "object")]
        public string Object { get; set; }

        [JsonProperty(PropertyName = "createdById")]
        public string CreatedById { get; set; }

        [JsonProperty(PropertyName = "createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty(PropertyName = "systemModstamp")]
        public DateTime SystemModstamp { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "concurrencyMode")]
        public string ConcurrencyMode { get; set; }

        [JsonProperty(PropertyName = "contentType")]
        public string ContentType { get; set; }

        [JsonProperty(PropertyName = "apiVersion")]
        public decimal ApiVersion { get; set; }

        [JsonProperty(PropertyName = "contentUrl")]
        public string ContentUrl { get; set; }

        [JsonProperty(PropertyName = "lineEnding")]
        public string LineEnding { get; set; }

        [JsonProperty(PropertyName = "columnDelimiter")]
        public string ColumnDelimiter { get; set; }
    }
}