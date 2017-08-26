using HashTag.Contracts.Repositories;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;
using HashTag.Domain.Models;

namespace HashTag.Data.Repositories
{
    [TransientDependency(ServiceType = typeof(IPredictionClassRepository))]
    internal class PredictionClassRepository : RepositoryBase<PredictionClass>, IPredictionClassRepository
    {
        public PredictionClassRepository(ApplicationDbContext dbContext, ICurrentUserAccessor currentUserAccessor)
            : base(dbContext, currentUserAccessor) { }
    }
}