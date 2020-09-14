using System;
using System.Threading.Tasks;
using AutoMapper;
using CoreApi.Contract;
using CoreApi.Contract.DatabaseContracts;
using CoreApi.Core.Persistence;
using CoreApi.Core.Service.Abstract;
using CoreApi.Entity.Models;

namespace CoreApi.Core.Service.Concrete
{
    public class UserService : ServiceBase<UserContract, User, IUserRepository>, IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IMapper mapper, IUserRepository userRepository) : base(mapper, userRepository)
        {
            _userRepository = userRepository;
        }
        
        public async Task<UserContract> GetAsync(string email)
        {
            var data = await _userRepository.GetAsync(email);
            return data == null ? null : Mapper.Map<UserContract>(data);
        }

        public override async Task<UserContract> UpdateAsync(UserContract contract)
        {
            var existUser = await _userRepository.GetAsync(contract.Email);
            if (existUser != null && contract.Id == existUser.Id)
                throw new ArgumentException("This email already exist.");
            
            contract.Password = BCrypt.Net.BCrypt.HashPassword(contract.Password);
            return await base.UpdateAsync(contract);
        }

        public override async Task<UserContract> AddAsync(UserContract contract)
        {
            if (await _userRepository.GetAsync(contract.Email) != null)
                throw new ArgumentException("This email already exist.");
            
            contract.Password = BCrypt.Net.BCrypt.HashPassword(contract.Password);
            return await base.AddAsync(contract);
        }

        public async Task<UserContract> IsVerified(LoginContract loginContract)
        {
            var user = await _userRepository.GetAsync(loginContract.Email);

            if (user == null)
                return null;
            
            var isVerified = BCrypt.Net.BCrypt.Verify(loginContract.Password, user.Password);
            return isVerified ? Mapper.Map<UserContract>(user) : null;
        }
    }
}