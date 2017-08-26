using HashTag.Contracts.Repositories;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;
using HashTag.Domain.Models;

namespace HashTag.Data.Repositories
{
    [TransientDependency(ServiceType = typeof(IClusterSamplePhotoRepository))]
    internal class ClusterSamplePhotoRepository : RepositoryBase<ClusterSamplePhoto>, IClusterSamplePhotoRepository
    {
        public ClusterSamplePhotoRepository(ApplicationDbContext dbContext, ICurrentUserAccessor currentUserAccessor)
            : base(dbContext, currentUserAccessor) { }
    }
}