namespace HashTag.Domain.Models
{
    public class PhotoHashTag
    {
        protected PhotoHashTag() {}

        public PhotoHashTag(Photo photo, HashTag hashTag)
        {
            Photo = photo;
            HashTag = hashTag;
        }

        public Photo Photo { get; set; }

        public HashTag HashTag { get; set; }
    }
}