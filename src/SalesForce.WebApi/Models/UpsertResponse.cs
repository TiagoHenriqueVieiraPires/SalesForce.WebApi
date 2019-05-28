using System;
using Newtonsoft.Json;
namespace SalesForce.WebApi.Models
{
    public class UpsertResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }
    }
}