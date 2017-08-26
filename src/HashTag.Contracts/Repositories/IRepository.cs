using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HashTag.Domain;

namespace HashTag.Contracts.Repositories
{
    public interface IRepository<in TKey, TEntity>
        where TEntity : EntityBase<TKey>
    {
        Task CreateAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(Expression<Func<TEntity, bool>> where);

        Task<TEntity> GetAsync(TKey key);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where);
        Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> where);
        Task<IEnumerable<TEntity>> GetAllAsync();
    }
}