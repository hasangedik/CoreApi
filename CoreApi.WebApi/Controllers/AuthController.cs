using System.Threading.Tasks;
using CoreApi.Contract;
using CoreApi.Contract.DatabaseContracts;
using CoreApi.Core.Service.Abstract;
using CoreApi.WebApi.Auth;
using CoreApi.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace CoreApi.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : CommonController
    {
        private readonly IUserService _userService;
        
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(LoginContract loginContract)
        {
            var badRequest = BadRequest("These credentials do not match our records.");

            var user = await _userService.IsVerified(loginContract);
            if (user == null)
                return badRequest;
            
            var jwtToken = JwtManager.GenerateToken(user);

            return Ok(new LoginResponseContract
            {
                AccessToken = jwtToken.RawData,
                ExpiresIn = jwtToken.Payload.Exp,
                TokenType = "JWT"
            });
        }
        
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(UserContract userContract)
        {
            return Ok(await _userService.AddAsync(userContract));
        }
    }
}