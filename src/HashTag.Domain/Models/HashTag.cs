using System.Collections.Generic;

namespace HashTag.Domain.Models
{
    public class HashTag : EntityBase<long>
    {
        protected HashTag() { }

        public HashTag(string name)
        {
            Name = name;
        }

        public string Name { get; protected set; }

        public ICollection<PhotoHashTag> PhotoHashTags { get; set; }
    }
}