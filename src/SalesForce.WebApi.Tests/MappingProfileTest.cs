using Xunit;
using System.Linq;
using System.Collections.Generic;
using SalesForce.WebApi.Models;
using SalesForce.WebApi.Models.ChatterDataTransferObject;
using AutoMapper;
using SalesForce.WebApi.Mappings;

namespace SalesForce.WebApi.Tests
{
    public class MappingProfileTest
    {
        private readonly IMapper _mapper;

        public MappingProfileTest()
        {
            _mapper = new MapperConfiguration(config => config.AddProfile<MappingProfile>()).CreateMapper();
        }

        [Fact]
        public void Test()
        {
            var requests = new List<ChatterRequest>()
            {
                new ChatterRequest()
                {
                    Text = "Chatter 1",
                    SubjectId = "C1"
                },
                new ChatterRequest()
                {
                    Text = "Chatter 2",
                    SubjectId = "C2"
                }
            };

            var result = _mapper.Map<List<FeedItemInput>>(requests);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            Assert.Equal("FeedItem", result.First().FeedElementType);
            Assert.Equal("C1", result.First().SubjectId);
            Assert.Equal("InternalUsers", result.First().Visibility);
            Assert.NotNull(result.First().Body);
            Assert.Single(result.First().Body.MessageSegments);
            Assert.Equal("Text", result.First().Body.MessageSegments.First().Type);
            Assert.Equal("Chatter 1", result.First().Body.MessageSegments.First().Text);

            Assert.Equal("FeedItem", result.Last().FeedElementType);
            Assert.Equal("C2", result.Last().SubjectId);
            Assert.Equal("InternalUsers", result.Last().Visibility);
            Assert.NotNull(result.Last().Body);
            Assert.Single(result.Last().Body.MessageSegments);
            Assert.Equal("Text", result.Last().Body.MessageSegments.First().Type);
            Assert.Equal("Chatter 2", result.Last().Body.MessageSegments.First().Text);
        }
    }
}