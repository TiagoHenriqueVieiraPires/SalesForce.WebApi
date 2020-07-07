using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Authorization
{
    public class ServerToServerAuthentication : BaseAuthorization
    {
        private readonly string _clientId;
        private readonly string _crmBaseUrl;
        private readonly string _clientSecret;
        private readonly string _grantType;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _securityToken;

        private AuthenticationResult AuthenticationResult;

        public ServerToServerAuthentication(string crmConnectionString)
        {
            var keys = GetConectionStringValues(crmConnectionString);
            _clientId = keys["clientId"];
            _clientSecret = keys["clientSecret"];
            _crmBaseUrl = keys["baseUrl"];
            _grantType = keys["grantType"];
            _userName = keys["userName"];
            _password = keys["password"];
            _securityToken = keys["securityToken"];
            AuthenticationResult = null;
        }

        public ServerToServerAuthentication(string clientId, string clientSecret, string baseUrl, string grantType, string userName, string password, string securityToken)
        {
            _clientId = clientId;
            _crmBaseUrl = baseUrl;
            _clientSecret = clientSecret;
            _grantType = grantType;
            _userName = userName;
            _password = password;
            _securityToken = securityToken;
            AuthenticationResult = null;
        }

        public override void RefreshCredentials()
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", GetAccessToken());
        }

        public string GetAccessToken()
        {
            if (AuthenticationResult != null && AuthenticationResult.IsValid())
                return $"{AuthenticationResult.TokenType} {AuthenticationResult.AccessToken}";

            RefreshAccessToken();
            return $"{AuthenticationResult.TokenType} {AuthenticationResult.AccessToken}";
        }

        public override string GetSystemUrl()
        {
            if (AuthenticationResult != null && AuthenticationResult.IsValid())
                return AuthenticationResult.InstanceUrl;

            RefreshAccessToken();
            return AuthenticationResult.InstanceUrl;
        }
        private void RefreshAccessToken()
        {
            var content = new StringContent("", Encoding.UTF8, "application/json");

            var result = httpClient.PostAsync(GetOauth2Url(), content).GetAwaiter().GetResult();
            result.EnsureSuccessStatusCode();

            string resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            AuthenticationResult = JsonConvert.DeserializeObject<AuthenticationResult>(resultContent);
        }

        public string GetCrmBaseUrl()
        {
            return _crmBaseUrl.ToLower();
        }

        private Dictionary<string, string> GetConectionStringValues(string crmConnectionString)
        {
            var keysValues = crmConnectionString.Split(';');
            var keysDictionary = new Dictionary<string, string>();

            foreach (var keyValue in keysValues)
            {
                var key = keyValue.Substring(0, keyValue.IndexOf('='));
                var value = keyValue.Substring(keyValue.IndexOf('=') + 1);
                keysDictionary.Add(key, value);
            }

            return keysDictionary;
        }

        private string GetOauth2Url()
        {
            return $"{_crmBaseUrl}/services/oauth2/token?grant_type={_grantType}&client_id={_clientId}&client_secret={_clientSecret}&username={_userName}&password={_password}{_securityToken}";
        }
    }
}