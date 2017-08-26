using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HashTag.Contracts.Repositories;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;
using HashTag.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HashTag.Data.Repositories
{
    [TransientDependency(ServiceType = typeof(IPhotoRepository))]
    internal class PhotoRepository : RepositoryBase<Photo>, IPhotoRepository
    {
        public PhotoRepository(ApplicationDbContext dbContext, ICurrentUserAccessor currentUserAccessor)
            : base(dbContext, currentUserAccessor) { }

        public async Task<Photo> GetWithUserAync(long id)
        {
            return await QueryAll()
                .Include(x => x.CreatedBy).ThenInclude(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Photo>> GetPagedAsync(int skip, int take)
        {
            return await QueryAll()
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.ApplicationUser)
                .Include(x => x.PhotoHashTags)
                .ThenInclude(x => x.HashTag)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Photo>> GetPagedByHashTagAsync(string hashTag, int skip, int take)
        {
            return await QueryAll()
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.ApplicationUser)
                .Include(x => x.PhotoHashTags)
                .ThenInclude(x => x.HashTag)
                .Where(x => x.PhotoHashTags.Any(y => y.HashTag.Name == hashTag))
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Photo>> GetPagedByDescriptionAsync(string description, int skip, int take)
        {
            return await QueryAll()
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.ApplicationUser)
                .Include(x => x.PhotoHashTags)
                .ThenInclude(x => x.HashTag)
                .Where(x => x.Description.Contains(description))
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Photo>> GetPagedByCluster(Cluster cluster, int skip, int take)
        {
            return await QueryAll()
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.ApplicationUser)
                .Include(x => x.PhotoHashTags)
                .ThenInclude(x => x.HashTag)
                .Where(x => x.Cluster == cluster)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Photo>> GetPagedByUserAsync(long userId, int skip, int take)
        {
            return await QueryAll()
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.ApplicationUser)
                .Include(x => x.PhotoHashTags).ThenInclude(x => x.HashTag)
                .Where(x => x.CreatedBy.Id == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }
}