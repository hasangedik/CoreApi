using AutoMapper;
using CoreApi.Contract.DatabaseContracts;
using CoreApi.Core.Persistence;
using CoreApi.Core.Service.Abstract;
using CoreApi.Entity.Models;

namespace CoreApi.Core.Service.Concrete
{
    public class NoteService : ServiceBase<NoteContract, Note, INoteRepository>, INoteService
    {
        public NoteService(IMapper mapper, INoteRepository noteRepository) : base(mapper, noteRepository)
        {
        }
    }
}