using System.Threading.Tasks;
using CoreApi.Entity.Models;

namespace CoreApi.Core.Persistence
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetAsync(string email);
    }
}
