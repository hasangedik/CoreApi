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
    public class UsersController : BaseController<IUserService, UserContract>
    {
        public UsersController(IUserService userService) : base(userService)
        {
        }
    }
}