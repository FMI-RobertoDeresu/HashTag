namespace HashTag.Domain.Models
{
    public class KMeansResearchResult : EntityBase<long>
    {
        protected KMeansResearchResult() { }

        public KMeansResearchResult(int numOfClusters, double result, long milliseconds, long clustersBuildMilliseconds, long searchMilliseconds)
        {
            Clusters = numOfClusters;
            Result = result;
            Milliseconds = milliseconds;
            ClustersBuildMilliseconds = clustersBuildMilliseconds;
            SearchMilliseconds = searchMilliseconds;
        }

        public int Clusters { get; protected set; }

        public double Result { get; protected set; }

        public long Milliseconds { get; protected set; }

        public long ClustersBuildMilliseconds { get; protected set; }

        public long SearchMilliseconds { get; protected set; }

        public override string ToString()
        {
            return $"{Clusters} clusters: {Result}";
        }
    }
}