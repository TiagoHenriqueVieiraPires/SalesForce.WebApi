
using Newtonsoft.Json;
using SalesForce.WebApi.Config;

namespace SalesForce.WebApi.Models
{
    public class Composite
    {
        [JsonProperty(PropertyName = "method")]
        public string Method { get; internal set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; internal set; }
        [JsonProperty(PropertyName = "referenceId")]
        public string ReferenceId { get; internal set; }
        [JsonProperty(PropertyName = "body")]
        public object Body { get; internal set; }

        /// <summary>
        /// Utilizado para insert, update, delete e upsesrt
        /// </summary>
        /// <param name="method">PATCH, DELETE, POST, GET</param>
        /// <param name="body">objeto a ser inserido no salesfroce</param>
        /// <param name="referenceId"> id de referencia de retorno</param>
        /// <param name="objectName">nome do objeto de destino no salesforce</param>
        /// <param name="recordIdName">nome da chave do registro</param>        
        public Composite(HttpMethods method, object body, string referenceId, string objectName, string recordIdName = "Id")
        {
            Method = method.ToString();
            ReferenceId = referenceId;

            switch (method)
            {
                case HttpMethods.PATCH:
                    ResponseRemoveKey responseRemoveKey;
                    responseRemoveKey = Helper.RemoveKeyToObject(body, recordIdName);
                    var key = responseRemoveKey.KeyValue;
                    Url = recordIdName == "Id" ? $"/services/data/v{ApiCofiguration.Version}/sobjects/{objectName}/{key}" :
                                                 $"/services/data/v{ApiCofiguration.Version}/sobjects/{objectName}/{recordIdName}/{key}";
                    Body = responseRemoveKey.Object;
                    break;

                case HttpMethods.DELETE:
                    Url = $"/services/data/v{ApiCofiguration.Version}/sobjects/{objectName}/{referenceId}";
                    break;

                case HttpMethods.POST:
                case HttpMethods.GET:
                    Url = $"/services/data/v{ApiCofiguration.Version}/sobjects/{objectName}";
                    Body = body;
                    break;

                default:
                    break;
            }
        }

    }
}