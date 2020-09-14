using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreApi.Contract;
using CoreApi.Entity;

namespace CoreApi.Core.Persistence
{
    public interface IGenericRepository<TEntity> where TEntity : class, IEntity, new()
    {
        Task<TEntity> GetAsync(object id);
        Task<IQueryable<TEntity>> GetAllAsync();
        Task<IQueryable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IQueryable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, int maxResult);
        Task<TEntity> AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<TEntity> UpdateAsync(IContract dataContract);
        Task RemoveAsync(object id);
        Task RemoveAsync(TEntity entity);
        Task RemoveRangeAsync(IEnumerable<TEntity> entities);
        Task<int> SaveChangesAsync();
    }
}
