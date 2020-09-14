using System.Threading.Tasks;
using CoreApi.Common.Extensions;
using CoreApi.Contract.DatabaseContracts;
using CoreApi.Core.Service.Abstract;
using CoreApi.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreApi.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class NotesController : BaseController<INoteService, NoteContract>
    {
        private readonly INoteService _noteService;
        public NotesController(INoteService noteService) : base(noteService)
        {
            _noteService = noteService;
        }

        public override Task<ActionResult> PostAsync(NoteContract contract)
        {
            contract.UserId = User.Identity.GetId();
            return base.PostAsync(contract);
        }
    }
}