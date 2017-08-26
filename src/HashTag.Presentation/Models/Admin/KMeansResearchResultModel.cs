using HashTag.Domain.AutoMapping;
using HashTag.Domain.Models;

namespace HashTag.Presentation.Models.Admin
{
    public class KMeansResearchResultModel : IMapFrom<KMeansResearchResult>
    {
        public int Clusters { get; set; }

        public double Result { get; set; }

        public long SearchMilliseconds { get; set; }
    }
}