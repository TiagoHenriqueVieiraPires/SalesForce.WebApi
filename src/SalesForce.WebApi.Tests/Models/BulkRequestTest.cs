using System.Collections.Generic;
using Newtonsoft.Json;
using SalesForce.WebApi.Models;
using Xunit;

namespace SalesForce.WebApi.Tests.Models
{
    public class BulkRequestTest
    {
        [Fact]
        public void Test()
        {
            var mockRequest = new Request() { Records = new List<object>() };

            mockRequest.Records.Add(

                    new Dictionary<object, object> {
                        { "attributes", new Dictionary<string, string> { {"type","Account"}, {"referenceId", "06ss76046d243"} } },
                        {"Name","coisa"},
                        {"Documento__c","065476046d243"}
                    }
                );


            var mockBulkRequest = new BulkCreateRequest() { Request = mockRequest };

            var context = JsonConvert.SerializeObject(mockBulkRequest.Request);
        }

    }
}