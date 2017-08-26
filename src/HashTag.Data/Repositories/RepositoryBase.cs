using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using HashTag.Contracts.Repositories;
using HashTag.Contracts.Services;
using HashTag.Domain;
using HashTag.Domain.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace HashTag.Data.Repositories
{
    [TransientDependency(ServiceType = typeof(IRepository<,>))]
    internal class RepositoryBase<TEntity> : IRepository<long, TEntity>
        where TEntity : EntityBase<long>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        protected readonly DbSet<TEntity> DbSet;

        public RepositoryBase(ApplicationDbContext dbContext, ICurrentUserAccessor currentUserAccessor)
        {
            _dbContext = dbContext;
            _currentUserAccessor = currentUserAccessor;
            DbSet = dbContext.Set<TEntity>();
        }

        public virtual async Task CreateAsync(TEntity entity)
        {
            (entity as Entity<long>)?.BeforeInsert(_currentUserAccessor.User);
            await DbSet.AddAsync(entity);
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            await Task.Delay(0);
            (entity as Entity<long>)?.BeforeUpdate(_currentUserAccessor.User);
            DbSet.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            await Task.Delay(0);
            if (entity is Entity<long>)
                (entity as Entity<long>).MarkAsDeleted(_currentUserAccessor.User);
            else
                DbSet.Remove(entity);
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> where)
        {
            await Task.Delay(0);
            var objects = QueryAll().Where(where).AsEnumerable();
            foreach (var obj in objects)
                await DeleteAsync(obj);
        }

        public virtual async Task<TEntity> GetAsync(long id)
        {
            return await QueryAll().FirstOrDefaultAsync(x => x.Id == id);
        }

        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where)
        {
            return await QueryAll().FirstOrDefaultAsync(where);
        }

        public virtual async Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> where)
        {
            return await QueryAll().Where(where).ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await QueryAll().ToListAsync();
        }

        protected IQueryable<TEntity> QueryAll()
        {
            return typeof(Entity<long>).IsAssignableFrom(typeof(TEntity))
                ? DbSet.Where(x => !(x as Entity<long>).IsDeleted)
                : DbSet;
        }
    }
}