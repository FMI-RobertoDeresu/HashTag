using System.Linq;

namespace HashTag.Domain.Models
{
    public class SamplePhoto : EntityBase<long>, IPhoto
    {
        protected SamplePhoto() { }

        public SamplePhoto(string name, string location, string description, double[] prediction)
        {
            Name = name;
            Location = location;
            Description = description;
            Prediction = prediction;
        }

        public string Name { get; protected set; }

        private string _prediction { get; set; }

        private double[] _predictionArray { get; set; }

        public string Location { get; protected set; }

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
    }
}