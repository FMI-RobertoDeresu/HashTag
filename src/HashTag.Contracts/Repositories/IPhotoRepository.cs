using System.Collections.Generic;
using System.Threading.Tasks;
using HashTag.Domain.Models;

namespace HashTag.Contracts.Repositories
{
    public interface IPhotoRepository : IRepository<long, Photo>
    {
        Task<Photo> GetWithUserAync(long id);
        Task<IEnumerable<Photo>> GetPagedAsync(int skip, int take);
        Task<IEnumerable<Photo>> GetPagedByHashTagAsync(string hashTag, int skip, int take);
        Task<IEnumerable<Photo>> GetPagedByDescriptionAsync(string description, int skip, int take);
        Task<IEnumerable<Photo>> GetPagedByCluster(Cluster cluster, int skip, int take);
        Task<IEnumerable<Photo>> GetPagedByUserAsync(long userId, int skip, int take);
    }
}