using System;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Models
{
    public class JobStatusResponse
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

        [JsonProperty(PropertyName = "externalIdFieldName")]
        public string ExternalIdFieldName { get; set; }

        [JsonProperty(PropertyName = "concurrencyMode")]
        public string ConcurrencyMode { get; set; }

        [JsonProperty(PropertyName = "contentType")]
        public string ContentType { get; set; }

        [JsonProperty(PropertyName = "apiVersion")]
        public decimal ApiVersion { get; set; }

        [JsonProperty(PropertyName = "jobType")]
        public string JobType { get; set; }

        [JsonProperty(PropertyName = "lineEnding")]
        public string LineEnding { get; set; }

        [JsonProperty(PropertyName = "columnDelimiter")]
        public string ColumnDelimiter { get; set; }
        [JsonProperty(PropertyName = "numberRecordsProcessed")]
        public int NumberRecordsProcessed { get; set; }
        [JsonProperty(PropertyName = "numberRecordsFailed")]
        public int NumberRecordsFailed { get; set; }
        [JsonProperty(PropertyName = "retries")]
        public int Retries { get; set; }

        [JsonProperty(PropertyName = "totalProcessingTime")]
        public int TotalProcessingTime { get; set; }

        [JsonProperty(PropertyName = "apiActiveProcessingTime")]
        public int ApiActiveProcessingTime { get; set; }

        [JsonProperty(PropertyName = "apexProcessingTime")]
        public int ApexProcessingTime { get; set; }
    }
}