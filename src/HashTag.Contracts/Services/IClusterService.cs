using System.Collections.Generic;
using System.Threading.Tasks;
using HashTag.Domain.Models;

namespace HashTag.Contracts.Services
{
    public interface IClusterService
    {
        Task<Cluster> GetNearestClusterAsync(double[] prediction);
        Task<Cluster> GetNearestClusterAsync(double[] prediction, IEnumerable<Cluster> clusters);

        Task SaveClustersAsync();
        Task<IEnumerable<Cluster>> BuildClustersAsync(int clusters, IEnumerable<SamplePhoto> trainSamples);
    }
}