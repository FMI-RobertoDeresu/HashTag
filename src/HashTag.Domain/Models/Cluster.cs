using System.Collections.Generic;
using System.Linq;

namespace HashTag.Domain.Models
{
    public class Cluster : EntityBase<long>
    {
        protected Cluster() { }

        public Cluster(double[] initialCorePrediction)
        {
            CorePrediction = initialCorePrediction;
            Photos = new List<ClusterSamplePhoto>();
        }

        private string _corePrediction { get; set; }

        private double[] _corePredictionArray { get; set; }

        public double[] CorePrediction
        {
            get
            {
                return _corePredictionArray ??
                       (_corePredictionArray =
                           _corePrediction.Split(',').Select(x => x.Trim()).Select(double.Parse).ToArray());
            }
            set { _corePrediction = string.Join(", ", value); }
        }

        public ICollection<ClusterSamplePhoto> Photos { get; private set; }
    }
}