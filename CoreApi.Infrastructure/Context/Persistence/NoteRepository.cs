using CoreApi.Core.Persistence;
using CoreApi.Entity.Models;

namespace CoreApi.Infrastructure.Context.Persistence
{
    public class NoteRepository : GenericRepository<Note>, INoteRepository
    {
        public NoteRepository(DataContext context) : base(context)
        {
        }
    }
}
