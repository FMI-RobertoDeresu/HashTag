using System.Threading.Tasks;
using HashTag.Domain.Models;

namespace HashTag.Contracts.Repositories
{
    public interface IUserRepository : IRepository<long, User>
    {
        Task<User> GetWithAppUserAsync(long id);
    }
}