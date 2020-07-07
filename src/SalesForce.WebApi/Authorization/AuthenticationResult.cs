using System;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Authorization
{
    public class AuthenticationResult
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "instance_url")]
        public string InstanceUrl { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = "issued_at")]
        public string IssuedAt { get; set; }

        [JsonProperty(PropertyName = "signature")]
        public string Signature { get; set; }

        public bool IsValid()
        {
            var nowUnixTimeStamp = ConvertToUnixTimestamp(DateTime.Now);
            return nowUnixTimeStamp < Convert.ToDouble(IssuedAt);
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

    }
}