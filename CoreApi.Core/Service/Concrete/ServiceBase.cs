using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CoreApi.Contract;
using CoreApi.Core.Persistence;
using CoreApi.Core.Service.Abstract;
using CoreApi.Entity;

namespace CoreApi.Core.Service.Concrete
{
    public abstract class ServiceBase<TContract, TEntity, TRepository> : IServiceBase<TContract>
        where TContract : IContract
        where TEntity : class, IEntity, new()
        where TRepository : IGenericRepository<TEntity>
    {
        protected readonly IMapper Mapper;
        private readonly IGenericRepository<TEntity> _repository;

        protected ServiceBase(IMapper mapper, TRepository repository)
        {
            Mapper = mapper;
            _repository = repository;
        }
        
        public virtual async Task<TContract> GetAsync(int id)
        {
            return Mapper.Map<TContract>(await _repository.GetAsync(id));
        }

        public virtual async Task<List<TContract>> GetAsync()
        {
            return Mapper.Map<List<TContract>>(await _repository.GetAllAsync());
        }

        public virtual async Task<TContract> AddAsync(TContract contract)
        {
            var entity = await _repository.AddAsync(Mapper.Map<TEntity>(contract));
            return Mapper.Map<TContract>(entity);
        }

        public virtual async Task<TContract> UpdateAsync(TContract contract)
        {
            var savedEntity = await _repository.UpdateAsync(Mapper.Map<TEntity>(contract));
            return Mapper.Map<TContract>(savedEntity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.RemoveAsync(id);
        }
    }
}