using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Tests
{
    public class MockHttpMessageHandler<T> : HttpMessageHandler
    {
        private readonly T _response;
        private readonly HttpStatusCode _statusCode;
        public string Input { get; private set; }
        public int NumberOfCalls { get; private set; }

        public MockHttpMessageHandler(T response, HttpStatusCode statusCode)
        {
            _response = response;
            _statusCode = statusCode;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            NumberOfCalls++;

            if (request.Content != null)
            {
                Input = await request.Content.ReadAsStringAsync();
            }
            
            return new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new StringContent(JsonConvert.SerializeObject(_response), Encoding.UTF8, "application/json")
            };
        }
    }
}