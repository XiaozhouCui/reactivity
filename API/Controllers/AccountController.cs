using System.Threading.Tasks;
using API.DTOs;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // api controller attribute
    [ApiController]
    // route: api/account/...
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        // inject UserManager and SignInManager
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            // initialise fields from parameters
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // login end point: will return an user DTO
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // get user object from database
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            // check if user exists
            if (user == null) return Unauthorized();
            // compare password and signin
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (result.Succeeded)
            {
                return new UserDto
                {
                    DisplayName = user.DisplayName,
                    Image = null,
                    Token = "This will be a token",
                    Username = user.UserName
                };

            }

            return Unauthorized();
        }
    }
}