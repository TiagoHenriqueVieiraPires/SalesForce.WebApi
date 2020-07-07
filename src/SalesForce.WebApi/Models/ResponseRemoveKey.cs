namespace SalesForce.WebApi.Models
{
    public class ResponseRemoveKey
    {
        public object Object { get; set; }
        public string KeyValue { get; set; }

        public ResponseRemoveKey() { }

        public ResponseRemoveKey(object obj, string keyValue)
        {
            Object = obj;
            KeyValue = keyValue;
        }
    }
}