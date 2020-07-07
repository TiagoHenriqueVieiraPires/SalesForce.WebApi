using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Models.ChatterDataTransferObject
{
    public class BatchCollectionInput
    {
        [JsonProperty(PropertyName = "inputs")]
        public List<RichInput> Inputs { get; set; }

        public BatchCollectionInput()
        {
            Inputs = new List<RichInput>();
        }
    }
}