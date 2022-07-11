using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticationModel userModel)
        {
            var user = _userRepo.Authenticate(userModel.Username, userModel.Password);

            if(user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthenticationModel userModel)
        {
            bool isUserNameUnique = _userRepo.IsUniqueUser(userModel.Username);

            if (!isUserNameUnique)
                return BadRequest(new { message = "Username already exists" });

            var user = _userRepo.Register(userModel.Username, userModel.Password);

            if(user == null)
            {
                return BadRequest(new { message = "Error during registration" });
            }

            return Ok(userModel.Username);     
        }
    }
}
