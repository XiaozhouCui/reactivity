using System.Threading.Tasks;
using API.DTOs;
using API.Services;
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
        private readonly TokenService _tokenService;
        // inject UserManager, SignInManager, TokenService
        public AccountController(
        UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, TokenService tokenService)
        {
            // initialise fields from parameters
            _tokenService = tokenService;
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
                    Token = _tokenService.CreateToken(user),
                    Username = user.UserName
                };
            }

            return Unauthorized();
        }
    }
}