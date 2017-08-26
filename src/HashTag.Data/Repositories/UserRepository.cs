using System.Linq;
using System.Threading.Tasks;
using HashTag.Contracts.Repositories;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;
using HashTag.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HashTag.Data.Repositories
{
    [TransientDependency(ServiceType = typeof(IUserRepository))]
    internal class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext, ICurrentUserAccessor currentUserAccessor)
            : base(dbContext, currentUserAccessor) { }

        public Task<User> GetWithAppUserAsync(long id)
        {
            var user = QueryAll().Include(x => x.ApplicationUser).FirstOrDefault(x => x.Id == id);
            return Task.FromResult(user);
        }
    }
}