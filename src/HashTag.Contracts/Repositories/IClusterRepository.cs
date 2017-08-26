using System.Threading.Tasks;
using HashTag.Domain.Models;

namespace HashTag.Contracts.Repositories
{
    public interface IClusterRepository : IRepository<long, Cluster>
    {
        Task<Cluster> GetWithPhotosAsync(long key);
    }
}