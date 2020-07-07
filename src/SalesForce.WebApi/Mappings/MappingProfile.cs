using System.Collections.Generic;
using AutoMapper;
using SalesForce.WebApi.Models;
using SalesForce.WebApi.Models.ChatterDataTransferObject;

namespace SalesForce.WebApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ChatterRequest, RichInput>()
                .ForMember(destination => destination.FeedItemInput, opt => opt.MapFrom(source => source));

            CreateMap<ChatterRequest, FeedItemInput>()
                .ForMember(destination => destination.FeedElementType, opt => opt.MapFrom(source => "FeedItem"))
                .ForMember(destination => destination.SubjectId, opt => opt.MapFrom(source => source.SubjectId))
                .ForMember(destination => destination.Visibility, opt => opt.MapFrom(source => "InternalUsers"))
                .ForMember(destination => destination.Body, opt => opt.MapFrom(source =>
                    new MessageInputBody()
                    {
                        MessageSegments = new List<Segments>()
                        {
                            new Segments()
                            {
                                Type = "Text",
                                Text = source.Text
                            }
                        }
                    }
                ));
        }
    }
}