using HashTag.Contracts.Repositories;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;
using HashTag.Domain.Models;

namespace HashTag.Data.Repositories
{
    [TransientDependency(ServiceType = typeof(IKMeansResearchResultRepository))]
    internal class KMeansResearchResultRepository : RepositoryBase<KMeansResearchResult>, IKMeansResearchResultRepository
    {
        public KMeansResearchResultRepository(ApplicationDbContext dbContext, ICurrentUserAccessor currentUserAccessor)
            : base(dbContext, currentUserAccessor) { }
    }
}