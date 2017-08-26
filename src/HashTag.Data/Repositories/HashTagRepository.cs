using HashTag.Contracts.Repositories;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;

namespace HashTag.Data.Repositories
{
    [TransientDependency(ServiceType = typeof(IHashTagRepository))]
    internal class HashTagRepository : RepositoryBase<Domain.Models.HashTag>, IHashTagRepository
    {
        public HashTagRepository(ApplicationDbContext dbContext, ICurrentUserAccessor currentUserAccessor)
            : base(dbContext, currentUserAccessor) { }
    }
}