using System.Threading.Tasks;
using HashTag.Contracts;
using HashTag.Domain.DependencyInjection;

namespace HashTag.Data
{
    [ScopedDependency(ServiceType = typeof(IUnitOfWork))]
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool WasCommited { get; private set; }

        public bool WasRollBacked { get; private set; }

        public bool IsCompleted => WasRollBacked || WasCommited;

        public Task CommitAsync()
        {
            if (WasCommited)
                return Task.FromResult(0);

            _dbContext.SaveChanges();
            _dbContext.Database.CurrentTransaction?.Commit();
            WasCommited = true;

            return Task.FromResult(0);
        }

        public Task RollbackAsync()
        {
            if (WasRollBacked)
                return Task.FromResult(0);

            _dbContext.Database.CurrentTransaction?.Rollback();
            WasRollBacked = true;

            return Task.FromResult(0);
        }
    }
}