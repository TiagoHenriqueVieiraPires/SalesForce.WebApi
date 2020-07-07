using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Models.ChatterDataTransferObject
{
    public class RichInput
    {
        [JsonProperty(PropertyName = "richInput")]
        public FeedItemInput FeedItemInput { get; set; }

    }
}