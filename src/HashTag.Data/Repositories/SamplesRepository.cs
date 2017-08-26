using HashTag.Contracts.Repositories;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;
using HashTag.Domain.Models;

namespace HashTag.Data.Repositories
{
    [TransientDependency(ServiceType = typeof(ISamplesRepository))]
    internal class SamplesRepository : RepositoryBase<SamplePhoto>, ISamplesRepository
    {
        public SamplesRepository(ApplicationDbContext dbContext, ICurrentUserAccessor currentUserAccessor)
            : base(dbContext, currentUserAccessor) { }
    }
}