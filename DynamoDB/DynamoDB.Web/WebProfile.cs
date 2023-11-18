using AutoMapper;
using DynamoDB.Domain.Entities;
using DynamoDB.Web.Models;

namespace DynamoDB.Web
{
    public class MyMappingProfile : Profile
    {
        public MyMappingProfile()
        {
            
            CreateMap<(string Address, string Mobile), DynamoItem>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Mobile));

            
        }
    }

}
