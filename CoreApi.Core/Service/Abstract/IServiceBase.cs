using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApi.Contract;

namespace CoreApi.Core.Service.Abstract
{
    public interface IServiceBase<TContract> where TContract : IContract
    {
        Task<TContract> GetAsync(int id);
        Task<List<TContract>> GetAsync();
        Task<TContract> AddAsync(TContract contract);
        Task<TContract> UpdateAsync(TContract contract);
        Task DeleteAsync(int id);
    }
}
