
using System;
using System.Net;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Models.ChatterDataTransferObject
{
    public class BatchResultItem : Response<FeedItem>
    {
        [JsonProperty(PropertyName = "statusCode")]
        public override HttpStatusCode HttpStatusCode { get; set; }

        [JsonProperty(PropertyName = "result")]
        public override dynamic Body { get; set; }
    }
}