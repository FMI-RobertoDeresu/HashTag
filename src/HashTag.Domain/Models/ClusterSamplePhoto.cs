namespace HashTag.Domain.Models
{
    public class ClusterSamplePhoto : EntityBase<long>
    {
        protected ClusterSamplePhoto() {}

        public ClusterSamplePhoto(SamplePhoto samplePhoto)
        {
            SamplePhoto = samplePhoto;
        }

        public SamplePhoto SamplePhoto { get; protected set; }

        public Cluster Cluster { get; set; }
    }
}