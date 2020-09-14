using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreApi.Contract;
using CoreApi.Core.Persistence;
using CoreApi.Entity;
using Microsoft.EntityFrameworkCore;

namespace CoreApi.Infrastructure.Context.Persistence
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        private readonly DataContext _databaseContext;
        private readonly DbSet<TEntity> _entities;

        protected GenericRepository(DataContext context)
        {
            _databaseContext = context;
            _entities = _databaseContext.Set<TEntity>();
        }

        public async Task<TEntity> GetAsync(object id)
        {
            return await _entities.FindAsync(id);
        }

        public virtual async Task<IQueryable<TEntity>> GetAllAsync()
        {
            return await Task.FromResult(_entities.AsNoTracking());
        }

        public async Task<IQueryable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Task.FromResult(_entities.Where(predicate));
        }

        public async Task<IQueryable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, int maxResult)
        {
            return await Task.FromResult(_entities.Where(predicate).Take(maxResult));
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            if (entity is IHasCreatedAt created)
                created.CreatedAt = DateTime.Now;

            if (entity is ISoftDelete delete)
                delete.IsDeleted = false;

            var data = await _entities.AddAsync(entity);
            await _databaseContext.SaveChangesAsync();
            return data.Entity;
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _entities.AddRangeAsync(entities);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity is IHasCreatedAt at)
                at.UpdatedAt = DateTime.Now;
            
            _databaseContext.Entry(entity).State = EntityState.Modified;
            var updateResult = _entities.Update(entity);
            return await Task.FromResult(updateResult.Entity);
        }

        public virtual Task<TEntity> UpdateAsync(IContract dataContract)
        {
            var existEntity = _entities
                .IgnoreQueryFilters()
                .FirstOrDefault(x=> x.Id == dataContract.Id);

            if (existEntity == null)
                throw new DbUpdateException("Entity not found in database.");

            if (existEntity is IHasCreatedAt at)
                at.UpdatedAt = DateTime.Now;

            _databaseContext.Entry(existEntity).CurrentValues.SetValues(dataContract);
            return Task.FromResult(existEntity);
        }

        public async Task RemoveAsync(object id)
        {
            var entity = await GetAsync(id);
            if (entity == null)
                throw new ArgumentException("Delete item not found in database.");

            await RemoveAsync(entity);
        }

        public async Task RemoveAsync(TEntity entity)
        {
            var deletedAtProperty = entity.GetType().GetProperty("IsDeleted");
            if (deletedAtProperty == null)
            {
                _entities.Remove(entity);
            }
            else
            {
                entity.GetType().GetProperty("IsDeleted")?.SetValue(entity, true);
                await UpdateAsync(entity);
            }
        }
        
        public async Task RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                await RemoveAsync(entity);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _databaseContext.SaveChangesAsync();
        }
    }
}
