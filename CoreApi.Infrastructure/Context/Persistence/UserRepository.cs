using System.Threading.Tasks;
using CoreApi.Core.Persistence;
using CoreApi.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreApi.Infrastructure.Context.Persistence
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly DbSet<User> _dbSet;
        public UserRepository(DataContext context) : base(context)
        {
            _dbSet = context.Set<User>();
        }

        public async Task<User> GetAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
