using System.Collections.Generic;
using HashTag.Domain.AutoMapping;
using HashTag.Domain.Dtos;

namespace HashTag.Presentation.Models.Photo
{
    public class PhotoModel : IMapFrom<PhotoDto>
    {
        public long Id { get; set; }

        public string User { get; set; }

        public string Address => $"/api/photos/get/{Id}";

        public string Url => Address; //used for lightbox

        public string Description { get; set; }

        public IEnumerable<string> HashTags { get; set; }

        public bool ShowActions { get; set; }
    }
}