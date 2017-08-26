using System.Linq;
using System.Threading.Tasks;
using HashTag.Contracts.Repositories;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;
using HashTag.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HashTag.Data.Repositories
{
    [TransientDependency(ServiceType = typeof(IClusterRepository))]
    internal class ClusterRepository : RepositoryBase<Cluster>, IClusterRepository
    {
        public ClusterRepository(ApplicationDbContext dbContext, ICurrentUserAccessor currentUserAccessor)
            : base(dbContext, currentUserAccessor) { }

        public async Task<Cluster> GetWithPhotosAsync(long key)
        {
            return await QueryAll()
                .Where(cluster => cluster.Id == key)
                .Include(cluster => cluster.Photos)
                .ThenInclude(photo => photo.SamplePhoto)
                .FirstOrDefaultAsync();
        }
    }
}