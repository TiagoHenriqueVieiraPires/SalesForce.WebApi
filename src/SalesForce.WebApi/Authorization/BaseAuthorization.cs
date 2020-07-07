using System;
using System.Net.Http;

namespace SalesForce.WebApi.Authorization
{
    public abstract class BaseAuthorization
    {
        protected HttpClient httpClient;
        protected HttpClientHandler handler;
        public TimeSpan Timeout { get; set; }

        public BaseAuthorization()
        {
            handler = new HttpClientHandler();
            handler.UseCookies = false;
            httpClient = new HttpClient(handler);
            Timeout = new TimeSpan(0, 2, 0);
        }

        public abstract void RefreshCredentials();

        public virtual HttpClient GetHttpCliente()
        {
            RefreshCredentials();
            return httpClient;
        }

        public void ConfigHttpClient()
        {
            if (!httpClient.DefaultRequestHeaders.Contains("Accept"))
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public abstract string GetSystemUrl();
    }
}