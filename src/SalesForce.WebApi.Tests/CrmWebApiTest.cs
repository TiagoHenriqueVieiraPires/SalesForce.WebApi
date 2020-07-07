using Xunit;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using SalesForce.WebApi.Authorization;
using SalesForce.WebApi.Interfaces;
using SalesForce.WebApi.Models;
using SalesForce.WebApi.Models.ChatterDataTransferObject;
using Moq;


namespace SalesForce.WebApi.Tests
{
    public class CrmWebApiTest
    {
        private Mock<ServerToServerAuthentication> MockServer;
        private ICrmWebApi CrmWebApi;

        public CrmWebApiTest()
        {
            MockServer = new Mock<ServerToServerAuthentication>("clientId=clientId;clientSecret=clientSecret;baseUrl=baseUrl;grantType=password;userName=userName;password=password;securityToken=securityToken");
            MockServer.Setup(config => config.GetSystemUrl()).Returns("https://system-url.com");
            CrmWebApi = new CrmWebApi(MockServer.Object);
        }

        #region Test MultipleCreateChatterAsync
        [Fact]
        public void MultipleCreateChatterAsync_Success()
        {
            // Arrange
            var mockRequests = new List<ChatterRequest>()
            {
                new ChatterRequest()
                {
                    Text = "Chatter 1 - Unit Test",
                    SubjectId = "001M000001EQdwFIAT"
                },
                new ChatterRequest()
                {
                    Text = "Chatter 2- Unit Test",
                    SubjectId = "001M000001EPqVjIAL"
                }
            };

            var mockResponses = new BatchResults()
            {
                HasErrors = false,
                Results = new List<BatchResultItem>()
                {
                    new BatchResultItem()
                    {
                        HttpStatusCode = HttpStatusCode.Created,
                        Body = new FeedItem()
                        {
                            Id = "C1",
                            Parent = new ParentFeedItem()
                            {
                                Id = "001M000001EQdwFIAT",
                                Type = "Account"
                            }
                        }

                    },
                    new BatchResultItem()
                    {
                        HttpStatusCode = HttpStatusCode.Created,
                        Body = new FeedItem()
                        {
                            Id = "C2",
                            Parent = new ParentFeedItem()
                            {
                                Id = "001M000001EPqVjIAL",
                                Type = "Account"
                            }
                        }
                    }
                }
            };

            var messageHandler = new MockHttpMessageHandler<BatchResults>(mockResponses, HttpStatusCode.OK);
            var httpClient = new HttpClient(messageHandler);
            MockServer.Setup(config => config.GetHttpCliente()).Returns(httpClient);

            //Action
            BatchResults retorno = CrmWebApi.MultipleCreateChatterAsync(mockRequests).GetAwaiter().GetResult();

            //Assert
            Assert.Equal(1, messageHandler.NumberOfCalls);
            Assert.NotNull(retorno);
            Assert.False(retorno.HasErrors);
            Assert.Equal(2, retorno.Results.Count());

            Assert.Equal(HttpStatusCode.Created, retorno.Results.First().HttpStatusCode);
            Assert.NotNull(retorno.Results.First().Sucess);
            Assert.Equal("C1", retorno.Results.First().Sucess.Id);
            Assert.NotNull(retorno.Results.First().Sucess.Parent);
            Assert.Equal("001M000001EQdwFIAT", retorno.Results.First().Sucess.Parent.Id);
            Assert.Equal("Account", retorno.Results.First().Sucess.Parent.Type);

            Assert.Equal(HttpStatusCode.Created, retorno.Results.Last().HttpStatusCode);
            Assert.NotNull(retorno.Results.Last().Sucess);
            Assert.Equal("C2", retorno.Results.Last().Sucess.Id);
            Assert.NotNull(retorno.Results.Last().Sucess.Parent);
            Assert.Equal("001M000001EPqVjIAL", retorno.Results.Last().Sucess.Parent.Id);
            Assert.Equal("Account", retorno.Results.Last().Sucess.Parent.Type);
        }

        [Fact]
        public void MultipleCreateChatterAsync_Error()
        {
            // Arrange
            var mockRequests = new List<ChatterRequest>()
            {
                new ChatterRequest()
                {
                    Text = "Chatter 1 - Unit Test",
                    SubjectId = "A"
                }
            };

            var mockResponses = new BatchResults()
            {
                HasErrors = true,
                Results = new List<BatchResultItem>()
                {
                    new BatchResultItem()
                    {
                        HttpStatusCode = HttpStatusCode.BadRequest,
                        Body = new List<ErroDetail>()
                        {
                            new ErroDetail()
                            {
                                ErrorCode = "INVALID_ID_FIELD",
                                Message = "Invalid parameter value &#39;a&#39; for parameter &#39;subjectId&#39;."
                            }
                        }
                    }
                }
            };

            var messageHandler = new MockHttpMessageHandler<BatchResults>(mockResponses, HttpStatusCode.OK);
            var httpClient = new HttpClient(messageHandler);
            MockServer.Setup(config => config.GetHttpCliente()).Returns(httpClient);

            //Action
            BatchResults retorno = CrmWebApi.MultipleCreateChatterAsync(mockRequests).GetAwaiter().GetResult();

            //Assert
            Assert.Equal(1, messageHandler.NumberOfCalls);
            Assert.NotNull(retorno);
            Assert.True(retorno.HasErrors);
            Assert.Single(retorno.Results);

            Assert.Equal(HttpStatusCode.BadRequest, retorno.Results.First().HttpStatusCode);
            Assert.NotNull(retorno.Results.First().Errors);
            Assert.Single(retorno.Results.First().Errors);
            Assert.Equal("INVALID_ID_FIELD", retorno.Results.First().Errors.First().ErrorCode);
            Assert.Equal("Invalid parameter value &#39;a&#39; for parameter &#39;subjectId&#39;.", retorno.Results.First().Errors.First().Message);
        }
        #endregion

        #region Test MultipleRequestAsync
        [Fact]
        public void MultipleRequestAsync_Success()
        {
            // Arrange
            var mockRequests = new CompositeRequest()
            {
                composites = new List<Composite>()
                {
                    new Composite(HttpMethods.PATCH, new { LastName = "Nome de Teste" }, "R1", "Account")
                }
            };

            var headers = new Dictionary<string, string>();
            headers.Add("Location", "/services/data/v48.0/sobjects/Account/R1");

            var mockResponses = new
            {
                compositeResponse = new List<CompositeResponse>()
                {
                    new CompositeResponse()
                    {
                        Body = new {
                            Id = "R1",
                            Success = true,
                            Errors = new List<ErroDetail>(),
                            Cretead = false
                        },
                        HttpHeaders = headers,
                        HttpStatusCode = HttpStatusCode.OK,
                        ReferenceId = "R1"
                    }
                }
            };

            var messageHandler = new MockHttpMessageHandler<object>(mockResponses, HttpStatusCode.OK);
            var httpClient = new HttpClient(messageHandler);
            MockServer.Setup(config => config.GetHttpCliente()).Returns(httpClient);

            //Action
            List<CompositeResponse> retorno = CrmWebApi.MultipleRequestAsync(mockRequests).GetAwaiter().GetResult();

            //Assert
            Assert.Equal(1, messageHandler.NumberOfCalls);
            Assert.NotNull(retorno);
            Assert.Single(retorno);

            Assert.Equal(HttpStatusCode.OK, retorno.First().HttpStatusCode);
            Assert.Equal("R1", retorno.First().ReferenceId);
            Assert.NotNull(retorno.First().HttpHeaders);
            Assert.Single(retorno.First().HttpHeaders);

            Assert.NotNull(retorno.First().Sucess);
            Assert.Equal("R1", retorno.First().Sucess.Id);
            Assert.True(retorno.First().Sucess.Success);
            Assert.Null(retorno.First().Errors);

            Assert.Equal("Location", retorno.First().HttpHeaders.First().Key);
            Assert.Equal("/services/data/v48.0/sobjects/Account/R1", retorno.First().HttpHeaders.First().Value);
        }

        [Fact]
        public void MultipleRequestAsync_Error()
        {
            // Arrange
            var mockRequests = new CompositeRequest()
            {
                composites = new List<Composite>()
                {
                    new Composite(HttpMethods.PATCH, new { Name = "Nome de Teste" }, "R1", "Account")
                }
            };

            var mockResponses = new
            {
                compositeResponse = new List<CompositeResponse>()
                {
                    new CompositeResponse()
                    {
                        Body = new List<object>() {
                            new {
                                Message = "Unable to create/update fields: Name.",
                                ErrorCode = "INVALID_FIELD_FOR_INSERT_UPDATE",
                                Fields = new List<string> { "Name" }
                            }
                        },
                        HttpHeaders = new Dictionary<string, string>(),
                        HttpStatusCode = HttpStatusCode.BadRequest,
                        ReferenceId = "R1"
                    }
                }
            };

            var messageHandler = new MockHttpMessageHandler<object>(mockResponses, HttpStatusCode.OK);
            var httpClient = new HttpClient(messageHandler);
            MockServer.Setup(config => config.GetHttpCliente()).Returns(httpClient);

            //Action
            List<CompositeResponse> retorno = CrmWebApi.MultipleRequestAsync(mockRequests).GetAwaiter().GetResult();

            //Assert
            Assert.Equal(1, messageHandler.NumberOfCalls);
            Assert.NotNull(retorno);
            Assert.Single(retorno);

            Assert.Equal(HttpStatusCode.BadRequest, retorno.First().HttpStatusCode);
            Assert.Equal("R1", retorno.First().ReferenceId);
            Assert.NotNull(retorno.First().HttpHeaders);
            Assert.Empty(retorno.First().HttpHeaders);
            Assert.Null(retorno.First().Sucess);

            Assert.NotNull(retorno.First().Errors);
            Assert.Single(retorno.First().Errors);
            Assert.Equal("Unable to create/update fields: Name.", retorno.First().Errors.First().Message);
            Assert.Equal("INVALID_FIELD_FOR_INSERT_UPDATE", retorno.First().Errors.First().ErrorCode);
            Assert.NotNull(retorno.First().Errors.First().Fields);
            Assert.Single(retorno.First().Errors.First().Fields);
            Assert.Equal("Name", retorno.First().Errors.First().Fields.First());
        }

        #endregion
    }
}