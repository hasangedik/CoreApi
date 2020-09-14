using System.Threading.Tasks;
using CoreApi.Contract;
using CoreApi.Contract.DatabaseContracts;

namespace CoreApi.Core.Service.Abstract
{
    public interface IUserService : IServiceBase<UserContract>
    {
        Task<UserContract> GetAsync(string email);
        Task<UserContract> IsVerified(LoginContract loginContract);
    }
}
