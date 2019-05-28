using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace SalesForce.WebApi.Models
{
    public class CompositeRequest
    {
        [JsonProperty(PropertyName = "allOrNone")]
        public bool AllOrNone { get; set; }
        [JsonProperty(PropertyName = "compositeRequest")]
        public List<Composite> composites { get; set; }

        public CompositeRequest(){
            composites = new List<Composite>();
        }
    }
}