using Newtonsoft.Json;

namespace SalesForce.WebApi.Models
{
    public class JobRequest
    {
        [JsonProperty(PropertyName = "object")]
        public string Object { get; set; }
        [JsonProperty(PropertyName = "contentType")]
        public string ContentType { get; set; }
        [JsonProperty(PropertyName = "operation")]
        public string Operation { get; set; }
        [JsonProperty(PropertyName = "lineEnding", NullValueHandling = NullValueHandling.Ignore)]
        public string LineEnding { get; set; }
        [JsonProperty(PropertyName = "externalIdFieldName", NullValueHandling = NullValueHandling.Ignore)]
        public string ExternalIdFieldName { get; set; }
        public JobRequest()
        {
        }
        public JobRequest(string _object, string _contentType, string _operation, string _lineEnding,string _externalIdFieldNam)
        {
            Object = _object;
            ContentType = _contentType;
            Operation = _operation;
            LineEnding = _lineEnding;
            ExternalIdFieldName=_externalIdFieldNam;
        }

    }
}