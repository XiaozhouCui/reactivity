using System.Threading.Tasks;
using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // api controller attributes
    [AllowAnonymous] // login/reg endpoints must NOT be protected by auth middleware
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

        // login end point: will return an user Data Transfer Object (DTO)
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

        // register end point: will return an user Data Transfer Object (DTO)
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // check username and email address availability
            if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
            {
                return BadRequest("Email taken");
            }
            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.Username))
            {
                return BadRequest("Username taken");
            }

            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Username
            };

            // create new user in DB
            var result = await _userManager.CreateAsync(user, registerDto.Password);

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

            // if the password is too weak, reg will fail

            return BadRequest("Problem registering user");
        }
    }
}