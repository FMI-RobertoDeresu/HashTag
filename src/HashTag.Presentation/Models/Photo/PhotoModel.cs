using System.Collections.Generic;
using AutoMapper;
using HashTag.Domain.AutoMapping;
using HashTag.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace HashTag.Presentation.Models.Photo
{
    public class PhotoModel : IAutoMapCustom
    {
        public long Id { get; set; }

        public string User { get; set; }

        public string UserUrl { get; set; }

        public string Address { get; private set; }

        public string Url => Address; //used for lightbox

        public string Description { get; set; }

        public IEnumerable<string> HashTags { get; set; }

        public bool ShowActions { get; set; }

        public void SetAddress(IUrlHelper urlHelper)
        {
            Address = urlHelper.Action("Get", "Photos", new {id = Id});
            UserUrl = urlHelper.Action("Profile", "Pages", new { userName = User });
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<PhotoDto, PhotoModel>()
                .ForMember(dest => dest.Address, map => map.Ignore())
                .ForMember(dest => dest.UserUrl, map => map.Ignore());
        }
    }
}