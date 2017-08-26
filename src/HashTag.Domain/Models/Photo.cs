using System.Collections.Generic;
using System.Linq;

namespace HashTag.Domain.Models
{
    public class Photo : Entity<long>, IPhoto
    {
        protected Photo()
        {
            _prediction = string.Empty;
        }

        public Photo(string name, string location, string description, double[] prediction, Cluster cluster)
        {
            Name = name;
            Location = location;
            Description = description;
            Prediction = prediction;
            Cluster = cluster;
            PhotoHashTags = new List<PhotoHashTag>();
        }

        private string _prediction { get; set; }

        private double[] _predictionArray { get; set; }

        public string Location { get; protected set; }

        public string Name { get; protected set; }

        public string Description { get; set; }

        public double[] Prediction
        {
            get
            {
                return _predictionArray ??
                       (_predictionArray = _prediction.Split(',').Select(x => x.Trim()).Select(double.Parse).ToArray());
            }
            set { _prediction = string.Join(", ", value); }
        }

        public Cluster Cluster { get; protected set; }

        public long ClusterId { get; set; }

        public ICollection<PhotoHashTag> PhotoHashTags { get; protected set; }

        public void BindToCluster(Cluster nearestCluster)
        {
            Cluster = nearestCluster;
        }
    }
}