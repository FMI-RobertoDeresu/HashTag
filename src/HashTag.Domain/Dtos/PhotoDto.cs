using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using HashTag.Domain.AutoMapping;
using HashTag.Domain.Models;

namespace HashTag.Domain.Dtos
{
    public class PhotoDto : IAutoMapCustom
    {
        public long Id { get; set; }

        public string User { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public IEnumerable<string> HashTags { get; set; }

        public bool ShowActions { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Photo, PhotoDto>()
                .ForMember(dest => dest.ShowActions, map => map.Ignore())
                .ForMember(dest => dest.User, map => map.MapFrom(
                    src => src.CreatedBy.ApplicationUser != null ? src.CreatedBy.ApplicationUser.UserName : string.Empty))
                .ForMember(dest => dest.HashTags, map => map.MapFrom(src => src.PhotoHashTags.Select(x => x.HashTag.Name)));
        }
    }
}